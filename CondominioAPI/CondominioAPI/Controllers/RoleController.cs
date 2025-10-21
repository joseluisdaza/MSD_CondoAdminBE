using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;

        public RoleController(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        /// <summary>
        /// Obtiene los roles de un usuario (RoleAdmin y Administrador)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.RoleAdmin},{AppRoles.Administrador}")]
        public async Task<ActionResult<IEnumerable<RoleRequest>>> GetRolesForUser(int id)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(id);
            if (user == null)
                return NotFound();

            var roles = user.UserRoles
                .Where(ur => ur.EndDate == null)
                .Select(ur => ur.Role.ToRoleRequest());
            return Ok(roles);
        }

        /// <summary>
        /// Asigna un rol a un usuario (Solo RoleAdmin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.RoleAdmin)]
        public async Task<IActionResult> AssignRoleToUser(UserRoleRequest userRoleRequest)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userRoleRequest.UserId);
            if (user == null)
                return NotFound("Usuario no encontrado.");
            var role = await _roleRepository.GetByIdAsync(userRoleRequest.RoleId);
            if (role == null)
                return NotFound("Rol no encontrado.");
            
            // Check if the user already has the role assigned and it's active
            var existingUserRole = user.UserRoles
                .FirstOrDefault(ur => ur.RoleId == userRoleRequest.RoleId && ur.EndDate == null);
            if (existingUserRole != null)
                return Ok(new { message = "El usuario ya tiene este rol asignado" });

            //Check if the user has the role assigned but it's inactive (EndDate is not null)
            existingUserRole = user.UserRoles
                .FirstOrDefault(ur => ur.RoleId == userRoleRequest.RoleId && ur.EndDate.HasValue);

            if (existingUserRole != null)
            {
                //Reactivate the role by setting EndDate to null
                existingUserRole.EndDate = null;
                existingUserRole.StartDate = DateTime.Now;
                await _userRoleRepository.UpdateAsync(existingUserRole);
                return Ok(new { message = "Rol reactivado exitosamente" });
            }

            // Assign the role to the user if the role is not already assigned
            var userRole = userRoleRequest.ToUserRole();
            userRole.StartDate = DateTime.Now;
            await _userRoleRepository.AddAsync(userRole);
            return Ok(new { message = "Rol asignado exitosamente" });
        }

        /// <summary>
        /// Remueve un rol de un usuario (Solo RoleAdmin)
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = AppRoles.RoleAdmin)]
        public async Task<IActionResult> RemoveRoleFromUser(UserRoleRequest userRoleRequest)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userRoleRequest.UserId);
            if (user == null)
                return NotFound("Usuario no encontrado.");
            
            var role = await _roleRepository.GetByIdAsync(userRoleRequest.RoleId);
            if (role == null)
                return NotFound("Rol no encontrado.");
            
            var userRole = user.UserRoles
                .FirstOrDefault(ur => ur.RoleId == userRoleRequest.RoleId && ur.EndDate == null);
            if (userRole == null)
                return NotFound("El usuario no tiene este rol asignado");

            // Set the EndDate to mark the role as inactive
            userRole.EndDate = DateTime.Now;
            await _userRoleRepository.UpdateAsync(userRole);
            return Ok(new { message = "Rol removido exitosamente" });
        }

        /// <summary>
        /// Obtiene todos los roles disponibles en el sistema (RoleAdmin y Administrador)
        /// </summary>
        [HttpGet("available")]
        [Authorize(Roles = $"{AppRoles.RoleAdmin},{AppRoles.Administrador}")]
        public async Task<ActionResult<IEnumerable<RoleRequest>>> GetAllRoles()
        {
            var roles = await _roleRepository.GetAllAsync();
            return Ok(roles.Select(r => r.ToRoleRequest()));
        }
    }
}
