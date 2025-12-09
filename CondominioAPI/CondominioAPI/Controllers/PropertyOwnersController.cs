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
    [Authorize]
    public class PropertyOwnersController : ControllerBase
    {
        private readonly IPropertyOwnerRepository _propertyOwnerRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;

        public PropertyOwnersController(
            IPropertyOwnerRepository propertyOwnerRepository,
            IPropertyRepository propertyRepository,
            IUserRepository userRepository)
        {
            _propertyOwnerRepository = propertyOwnerRepository;
            _propertyRepository = propertyRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Obtiene relaciones de propietarios con filtros opcionales
        /// - Administradores/Directores/Super: Pueden ver cualquier propiedad
        /// - Habitantes: Solo pueden ver sus propias propiedades
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<PropertyOwnerResponse>>> GetFiltered(
            [FromQuery] int? propertyId,
            [FromQuery] int? userId,
            [FromQuery] bool includeFinalized = false)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.IsInRole(AppRoles.Administrador) || User.IsInRole(AppRoles.Director) || User.IsInRole(AppRoles.Super);

                Log.Information("GET > PropertyOwners > GetFiltered. User: {0}, PropertyId: {1}, UserId: {2}, IncludeFinalized: {3}", 
                    User.Identity?.Name, propertyId, userId, includeFinalized);

                IEnumerable<Condominio.Models.PropertyOwner> propertyOwners;

                if (isAdmin)
                {
                    // Administradores pueden ver cualquier propiedad
                    if (propertyId.HasValue || userId.HasValue)
                    {
                        propertyOwners = await _propertyOwnerRepository.GetFilteredAsync(propertyId, userId, includeFinalized);
                    }
                    else
                    {
                        propertyOwners = await _propertyOwnerRepository.GetAllWithRelationsAsync(includeFinalized);
                    }
                }
                else
                {
                    // Habitantes solo pueden ver sus propias propiedades
                    if (userId.HasValue && userId.Value != currentUserId)
                    {
                        return Forbid("No tienes permisos para ver las propiedades de otros usuarios");
                    }

                    // Para habitantes, siempre filtrar por su propio userId
                    propertyOwners = await _propertyOwnerRepository.GetFilteredAsync(propertyId, currentUserId, includeFinalized);
                }

                var responses = propertyOwners.Select(po => po.ToPropertyOwnerResponse()).ToList();
                
                Log.Information("PropertyOwners GetFiltered completed. Count: {0}", responses.Count);
                
                return Ok(responses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting filtered property owners");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una relación específica por PropertyId y UserId
        /// </summary>
        [HttpGet("{propertyId}/{userId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<PropertyOwnerResponse>> GetByPropertyAndUserId(int propertyId, int userId)
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.IsInRole(AppRoles.Administrador) || User.IsInRole(AppRoles.Director) || User.IsInRole(AppRoles.Super);

                Log.Information("GET > PropertyOwners > ByPropertyAndUserId. User: {0}, PropertyId: {1}, UserId: {2}", 
                    User.Identity?.Name, propertyId, userId);

                // Validar permisos
                if (!isAdmin && userId != currentUserId)
                {
                    return Forbid("No tienes permisos para ver las propiedades de otros usuarios");
                }

                var propertyOwner = await _propertyOwnerRepository.GetByPropertyAndUserIdAsync(propertyId, userId);
                if (propertyOwner == null)
                {
                    return NotFound("Relación propiedad-usuario no encontrada");
                }

                return Ok(propertyOwner.ToPropertyOwnerResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting property owner by PropertyId: {0}, UserId: {1}", propertyId, userId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva relación propiedad-propietario (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<PropertyOwnerResponse>> Create([FromBody] PropertyOwnerRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > PropertyOwners > Create. User: {0}, PropertyId: {1}, UserId: {2}", 
                    User.Identity?.Name, request.PropertyId, request.UserId);

                // Validar que la propiedad existe
                var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
                if (property == null)
                {
                    return BadRequest("La propiedad especificada no existe");
                }

                // Validar que el usuario existe
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    return BadRequest("El usuario especificado no existe");
                }

                // Validar que no existe una relación activa
                var existingRelation = await _propertyOwnerRepository.GetByPropertyAndUserIdAsync(request.PropertyId, request.UserId);
                if (existingRelation != null && existingRelation.EndDate == null)
                {
                    return BadRequest("Ya existe una relación activa entre esta propiedad y este usuario");
                }

                // Si existe una relación finalizada, crear una nueva entrada
                var propertyOwner = request.ToPropertyOwner();
                await _propertyOwnerRepository.AddAsync(propertyOwner);
                
                Log.Information("PropertyOwner relation created successfully. PropertyId: {0}, UserId: {1}", 
                    request.PropertyId, request.UserId);

                // Obtener la relación creada con las relaciones incluidas
                var createdRelation = await _propertyOwnerRepository.GetByPropertyAndUserIdAsync(request.PropertyId, request.UserId);
                
                return CreatedAtAction(nameof(GetByPropertyAndUserId), 
                    new { propertyId = request.PropertyId, userId = request.UserId }, 
                    createdRelation?.ToPropertyOwnerResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating property owner relation");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Finaliza una relación propiedad-propietario (Soft Delete) (Solo Administradores)
        /// </summary>
        [HttpDelete("{propertyId}/{userId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> Delete(int propertyId, int userId)
        {
            try
            {
                Log.Information("DELETE > PropertyOwners > Delete. User: {0}, PropertyId: {1}, UserId: {2}", 
                    User.Identity?.Name, propertyId, userId);

                var propertyOwner = await _propertyOwnerRepository.GetByPropertyAndUserIdAsync(propertyId, userId);
                if (propertyOwner == null)
                {
                    return NotFound("Relación propiedad-usuario no encontrada");
                }

                if (propertyOwner.EndDate != null)
                {
                    return BadRequest("La relación ya está finalizada");
                }

                // Realizar soft delete
                propertyOwner.EndDate = DateTime.Now;
                await _propertyOwnerRepository.UpdateAsync(propertyOwner);
                
                Log.Information("PropertyOwner relation finalized successfully. PropertyId: {0}, UserId: {1}", 
                    propertyId, userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting property owner relation. PropertyId: {0}, UserId: {1}", propertyId, userId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}