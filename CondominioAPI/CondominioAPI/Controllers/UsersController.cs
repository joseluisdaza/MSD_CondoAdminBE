using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Obtiene todos los usuarios (Solo Administradores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<UserBaseRequest>>> GetAll()
        {
            Log.Information("GET > User > GetAll. User: {0}", this.User.Identity.Name);
            var users = await _userRepository.GetAllAsync();
            return Ok(users.Select(u => u.ToUserBaseRequest()).ToList());
        }

        /// <summary>
        /// Obtiene un usuario por ID
        /// - Administradores: pueden ver cualquier usuario
        /// - Habitantes: solo pueden ver su propia información
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<UserBaseRequest>> GetById(int id)
        {
            Log.Information("GET > User > ById. User: {0}, Id: {1}", this.User.Identity.Name, id);

            // Obtener el ID del usuario autenticado
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole(AppRoles.Administrador);

            // Si no es administrador, solo puede ver su propia información
            if (!isAdmin && currentUserId != id)
            {
                return Forbid(); // 403 Forbidden
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user.ToUserBaseRequest());
        }

        /// <summary>
        /// Crea un nuevo usuario (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Create(NewUserRequest user)
        {
            Log.Information("POST > User > User: {0}", this.User.Identity.Name);

            if (user == null)
                return BadRequest("El usuario no puede ser nulo.");

            // Validar modelo automáticamente (incluyendo StrongPassword)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            Condominio.Models.User userEntity = user.ToUser();
            await _userRepository.AddAsync(userEntity);
            var createdUser = await _userRepository.GetByLoginAsync(user.Login);
            return Ok(createdUser.ToUserRequest(includeId: true));
        }

        /// <summary>
        /// Actualiza un usuario existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Update(int id, NewUserBaseRequest user)
        {
            Log.Information("PUT > User > User: {0}, Id: {1}", this.User.Identity.Name, id);

            var userFound = await _userRepository.GetByIdAsync(id);
            if (userFound == null)
                return NotFound();

            userFound.UpdateDataNewUserBaseRequest(user);
            await _userRepository.UpdateAsync(userFound);
            return Ok();
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario
        /// - Administradores: pueden cambiar contraseña de cualquier usuario
        /// - Habitantes: solo pueden cambiar su propia contraseña
        /// </summary>
        [HttpPatch("{id}/password")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordRequest passwordRequest)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUserName = User.Identity?.Name ?? "Unknown";
                
                Log.Information("PUT > User > UpdatePassword. User: {0}, TargetId: {1}", currentUserName, id);

                if (passwordRequest == null)
                {
                    Log.Warning("Password change failed - Null request for user ID {0} by user {1}", id, currentUserName);
                    return BadRequest("La solicitud de actualización de contraseña no puede ser nula.");
                }

                // Validar modelo automáticamente (incluyendo StrongPassword)
                if (!ModelState.IsValid)
                {
                    Log.Warning("Password change failed - Invalid format for user ID {0} by user {1}", id, currentUserName);
                    return BadRequest(ModelState);
                }

                // Obtener el ID del usuario autenticado
                var isAdmin = User.IsInRole(AppRoles.Administrador) || User.IsInRole(AppRoles.Super);

                // Si no es administrador, solo puede cambiar su propia contraseña
                if (!isAdmin && currentUserId != id)
                {
                    Log.Warning("Access denied - User {0} (ID: {1}) attempted to change password for user ID {2}", 
                        currentUserName, currentUserId, id);
                    return Forbid("No tienes permisos para cambiar la contraseña de otro usuario.");
                }

                var userFound = await _userRepository.GetByIdAsync(id);
                if (userFound == null)
                {
                    Log.Warning("Password change failed - User not found for ID {0}", id);
                    return NotFound("Usuario no encontrado.");
                }

                // Actualizar la contraseña
                userFound.Password = PasswordHasher.HashPassword(passwordRequest.NewPassword);
                await _userRepository.UpdateAsync(userFound);

                // Log de auditoría exitoso
                var actionMessage = currentUserId == id 
                    ? $"User changed their own password" 
                    : $"Admin {currentUserName} changed password for user {userFound.Login} (ID: {id})";
                    
                Log.Information("Password updated successfully - {0}", actionMessage);
                
                return Ok(new { message = "Contraseña actualizada exitosamente." });
            }
            catch (Exception ex)
            {
                var currentUserName = User.Identity?.Name ?? "Unknown";
                Log.Error(ex, "Error updating password for user ID {0} by user {1}", id, currentUserName);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un usuario (Soft Delete) (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Delete(int id)
        {
            Log.Information("DELETE > User >  User: {0}, Id: {1}", this.User.Identity.Name, id);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.EndDate = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return Ok();
        }
    }
}
