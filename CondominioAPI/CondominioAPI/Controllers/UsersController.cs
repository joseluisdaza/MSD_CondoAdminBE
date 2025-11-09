using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<UserRequest>> Create(UserRequest user)
        {
            // Validar modelo automáticamente (incluyendo StrongPassword)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.EndDate = null;
            var userEntity = user.ToUser();
            await _userRepository.AddAsync(userEntity);
            
            var createdUserRequest = userEntity.ToUserRequest(includeId: true);
            return CreatedAtAction(nameof(GetById), new { id = userEntity.Id }, createdUserRequest);
        }

        /// <summary>
        /// Actualiza un usuario existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Update(int id, UserRequest user)
        {
            var userFound = await _userRepository.GetByIdAsync(id);
            if (userFound == null)
                return NotFound();

            user.EndDate = userFound.EndDate;
            await _userRepository.UpdateAsync(user.ToUser());
            return Ok();
        }

        /// <summary>
        /// Elimina un usuario (Soft Delete) (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.EndDate = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return Ok();
        }
    }
}
