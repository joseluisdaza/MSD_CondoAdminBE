using Condominio.Models;
using Condominio.Repository.Repositories;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleSetupController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRepository _userRepository;

        public RoleSetupController(
            IRoleRepository roleRepository, 
            IUserRoleRepository userRoleRepository,
            IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Crea los roles por defecto del sistema si no existen
        /// ?? Este endpoint debería protegerse o eliminarse en producción
        /// </summary>
        [HttpPost("initialize-roles")]
        [AllowAnonymous] // Cambiar a [Authorize(Roles = AppRoles.RoleAdmin)] en producción
        public async Task<IActionResult> InitializeRoles()
        {
            try
            {
                var existingRoles = await _roleRepository.GetAllAsync();
                var createdRoles = new List<string>();

                // Definir roles a crear
                var rolesToCreate = new[]
                {
                    new { Name = AppRoles.Default, Description = "Rol por defecto para operaciones básicas (health check, login)" },
                    new { Name = AppRoles.Habitante, Description = "Residente que puede ver su información personal y propiedades asignadas" },
                    new { Name = AppRoles.Administrador, Description = "Administrador del sistema con permisos completos de CRUD en Usuarios y Propiedades" },
                    new { Name = AppRoles.RoleAdmin, Description = "Administrador de roles con permisos para gestionar roles de usuarios" }
                };

                foreach (var roleInfo in rolesToCreate)
                {
                    // Verificar si el rol ya existe
                    if (!existingRoles.Any(r => r.RolName == roleInfo.Name))
                    {
                        var newRole = new Role
                        {
                            RolName = roleInfo.Name,
                            Description = roleInfo.Description
                        };
                        await _roleRepository.AddAsync(newRole);
                        createdRoles.Add(roleInfo.Name);
                    }
                }

                if (createdRoles.Any())
                {
                    return Ok(new
                    {
                        message = "Roles creados exitosamente",
                        createdRoles = createdRoles,
                        totalRoles = (await _roleRepository.GetAllAsync()).Count()
                    });
                }

                return Ok(new
                {
                    message = "Todos los roles ya existen",
                    existingRoles = existingRoles.Select(r => r.RolName).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al crear roles",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Asigna rol de Administrador al primer usuario del sistema
        /// ?? Solo para configuración inicial - ELIMINAR en producción
        /// </summary>
        [HttpPost("setup-first-admin")]
        [AllowAnonymous] // Cambiar a protegido o eliminar en producción
        public async Task<IActionResult> SetupFirstAdmin()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var firstUser = users.FirstOrDefault();

                if (firstUser == null)
                {
                    return BadRequest(new { message = "No hay usuarios en el sistema. Crea un usuario primero." });
                }

                var roles = await _roleRepository.GetAllAsync();
                var adminRole = roles.FirstOrDefault(r => r.RolName == AppRoles.Administrador);
                var roleAdminRole = roles.FirstOrDefault(r => r.RolName == AppRoles.RoleAdmin);

                if (adminRole == null || roleAdminRole == null)
                {
                    return BadRequest(new { message = "Los roles no existen. Ejecuta /initialize-roles primero." });
                }

                var userWithRoles = await _userRepository.GetByIdWithRolesAsync(firstUser.Id);
                var assignedRoles = new List<string>();

                // Asignar rol Administrador si no lo tiene
                if (!userWithRoles.UserRoles.Any(ur => ur.RoleId == adminRole.Id && ur.EndDate == null))
                {
                    var userRoleAdmin = new UserRole
                    {
                        UserId = firstUser.Id,
                        RoleId = adminRole.Id,
                        StartDate = DateTime.Now
                    };
                    await _userRoleRepository.AddAsync(userRoleAdmin);
                    assignedRoles.Add(AppRoles.Administrador);
                }

                // Asignar rol RoleAdmin si no lo tiene
                if (!userWithRoles.UserRoles.Any(ur => ur.RoleId == roleAdminRole.Id && ur.EndDate == null))
                {
                    var userRoleRoleAdmin = new UserRole
                    {
                        UserId = firstUser.Id,
                        RoleId = roleAdminRole.Id,
                        StartDate = DateTime.Now
                    };
                    await _userRoleRepository.AddAsync(userRoleRoleAdmin);
                    assignedRoles.Add(AppRoles.RoleAdmin);
                }

                if (assignedRoles.Any())
                {
                    return Ok(new
                    {
                        message = "Roles de administrador asignados exitosamente",
                        user = new
                        {
                            id = firstUser.Id,
                            login = firstUser.Login,
                            userName = firstUser.UserName
                        },
                        assignedRoles = assignedRoles
                    });
                }

                return Ok(new
                {
                    message = "El usuario ya tiene los roles de administrador",
                    user = new
                    {
                        id = firstUser.Id,
                        login = firstUser.Login,
                        userName = firstUser.UserName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al asignar roles de administrador",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lista todos los roles del sistema
        /// </summary>
        [HttpGet("list-roles")]
        [AllowAnonymous]
        public async Task<IActionResult> ListRoles()
        {
            var roles = await _roleRepository.GetAllAsync();
            return Ok(new
            {
                totalRoles = roles.Count(),
                roles = roles.Select(r => new
                {
                    id = r.Id,
                    name = r.RolName,
                    description = r.Description
                })
            });
        }
    }
}
