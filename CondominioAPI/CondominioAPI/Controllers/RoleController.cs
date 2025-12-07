using Condominio.DTOs;
using Condominio.Models;
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
        [HttpGet("{userId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<RoleRequest>>> GetRolesForUser(int userId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null)
                return NotFound();

            var roles = user.UserRoles
                .Where(ur => ur.EndDate == null)
                .Select(ur => ur.Role.ToRoleRequest());
            return Ok(roles);
        }

        [HttpPut("user/{userId}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> AssignRoleToUser(int userId, [FromBody] List<int> roleIds)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            // Validate all roles exist
            var roles = new List<Role>();
            foreach (var roleId in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    return NotFound($"Rol con ID {roleId} no encontrado.");
                roles.Add(role);
            }

            // Get currently active user roles
            var currentActiveRoles = user.UserRoles
                .Where(ur => ur.EndDate == null)
                .ToList();

            // Deactivate roles that are not in the new list
            foreach (var currentRole in currentActiveRoles)
            {
                if (!roleIds.Contains(currentRole.RoleId))
                {
                    currentRole.EndDate = DateTime.Now;
                    await _userRoleRepository.UpdateAsync(currentRole);
                }
            }

            // Activate or create roles from the new list
            foreach (var roleId in roleIds)
            {
                // Check if user already has this role active
                var existingActiveRole = user.UserRoles
                    .FirstOrDefault(ur => ur.RoleId == roleId && ur.EndDate == null);
                
                if (existingActiveRole != null)
                    continue; // Role is already active

                // Check if user had this role but it's inactive
                var existingInactiveRole = user.UserRoles
                    .FirstOrDefault(ur => ur.RoleId == roleId && ur.EndDate.HasValue);

                if (existingInactiveRole != null)
                {
                    // Reactivate the role
                    existingInactiveRole.EndDate = null;
                    existingInactiveRole.StartDate = DateTime.Now;
                    await _userRoleRepository.UpdateAsync(existingInactiveRole);
                }
                else
                {
                    // Create new role assignment
                    var newUserRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        StartDate = DateTime.Now,
                        EndDate = null
                    };
                    await _userRoleRepository.AddAsync(newUserRole);
                }
            }

            return Ok(new { message = "Roles actualizados exitosamente" });
        }

        /// <summary>
        /// Obtiene todos los roles disponibles en el sistema (RoleAdmin y Administrador)
        /// </summary>
        [HttpGet()]
        [Authorize(Roles = $"{AppRoles.Super},{AppRoles.Administrador}")]
        public async Task<ActionResult<IEnumerable<RoleRequest>>> GetAllRoles()
        {
            var roles = await _roleRepository.GetAllAsync();
            return Ok(roles.Select(r => r.ToRoleRequest()));
        }

    }
}
