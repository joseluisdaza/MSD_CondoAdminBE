using Condominio.Repository.Repositories;
using CondominioAPI.DTOs;
using CondominioAPI.Utils;
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

        [HttpGet("{id}")]
        [Authorize]
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

        [HttpPost]
        [Authorize]
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
                return Ok();

            //Check if the user has the role assigned but it's inactive (EndDate is not null)
            existingUserRole = user.UserRoles
                .FirstOrDefault(ur => ur.RoleId == userRoleRequest.RoleId && ur.EndDate.HasValue);

            if (existingUserRole != null)
            {
                //Reactivate the role by setting EndDate to null
                existingUserRole.EndDate = null;
                existingUserRole.StartDate = DateTime.Now;
                await _userRoleRepository.UpdateAsync(existingUserRole);
                return Ok();
            }

            // Assign the role to the user if the role is not already assigned
            var userRole = userRoleRequest.ToUserRole();
            await _userRoleRepository.AddAsync(userRole);
            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveRoleFromUser(UserRoleRequest userRoleRequest)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userRoleRequest.UserId);
            if (user == null)
                return NotFound("Ususario no encontrado.");
            
            var role = await _roleRepository.GetByIdAsync(userRoleRequest.RoleId);
            if (role == null)
                return NotFound("Rol no encontrado.");
            
            var userRole = user.UserRoles
                .FirstOrDefault(ur => ur.RoleId == userRoleRequest.RoleId && ur.EndDate == null);
            if (userRole == null)
                return Ok();

            // Set the EndDate to mark the role as inactive
            userRole.EndDate = DateTime.Now;
            await _userRoleRepository.UpdateAsync(userRole);
            return Ok();
        }
    }
}
