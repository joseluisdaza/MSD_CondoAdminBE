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
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPropertyOwnerRepository _propertyOwnerRepository;

        public PropertyController(IPropertyRepository propertyRepository, IUserRepository userRepository, IPropertyOwnerRepository propertyOwnerRepository)
        {
            _propertyRepository = propertyRepository;
            _userRepository = userRepository;
            _propertyOwnerRepository = propertyOwnerRepository;
        }

        /// <summary>
        /// Obtiene todas las propiedades (Solo Administradores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<ActionResult<IEnumerable<PropertyRequest>>> GetAll()
        {
            Log.Information("GET > Property > GetAll. User: {0}", this.User.Identity.Name);
            var properties = await _propertyRepository.GetAllAsync();
            return Ok(properties.Select(x => x.ToFullPropertyRequest()));
        }

        /// <summary>
        /// Obtiene los IDs de todas las propiedades asignadas a un usuario
        /// </summary>
        [HttpGet("UserPropertyIds")]
        [Authorize(Roles = $"{AppRoles.Habitante},{AppRoles.Administrador}")]
        public async Task<ActionResult<IEnumerable<int>>> GetUserPropertyIds()
        {
            Log.Information("GET > Property > UserPropertyIds > User: {0}", this.User.Identity.Name);

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (currentUserId == 0)
                return BadRequest("Usuario no identificado");

            try
            {
                // Obtener las propiedades asignadas al usuario (solo activas)
                var propertyOwners = await _propertyOwnerRepository.GetByUserIdAsync(currentUserId, includeFinalized: false);

                // Extraer solo los IDs de las propiedades
                var propertyIds = propertyOwners.Select(po => po.PropertyId).ToList();

                Log.Information("User {UserId} has {PropertyCount} properties assigned", currentUserId, propertyIds.Count);

                return Ok(propertyIds);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting property IDs for user {UserId}", currentUserId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los IDs de todas las propiedades asignadas a un usuario específico (Solo Administradores)
        /// </summary>
        [HttpGet("UserPropertyIds/{userId}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<ActionResult<IEnumerable<int>>> GetUserPropertyIdsByUserId(int userId)
        {
            Log.Information("GET > Property > UserPropertyIds > Admin User: {0}, Target UserId: {1}", this.User.Identity.Name, userId);

            try
            {
                // Verificar que el usuario existe
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return NotFound($"Usuario con ID {userId} no encontrado");

                // Obtener las propiedades asignadas al usuario especificado (solo activas)
                var propertyOwners = await _propertyOwnerRepository.GetByUserIdAsync(userId, includeFinalized: false);

                // Extraer solo los IDs de las propiedades
                var propertyIds = propertyOwners.Select(po => po.PropertyId).ToList();

                Log.Information("User {UserId} has {PropertyCount} properties assigned", userId, propertyIds.Count);

                return Ok(propertyIds);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting property IDs for user {UserId}", userId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene propiedades asignadas al usuario actual (Habitantes)
        /// o todas si es Administrador
        /// </summary>
        [HttpGet("ByUser")]
        [Authorize(Roles = $"{AppRoles.Habitante},{AppRoles.Administrador}")]
        public async Task<ActionResult<IEnumerable<PropertyRequest>>> GetMyProperties()
        {
            Log.Information("GET > Property > ByUser > User: {0}", this.User.Identity.Name);
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole(AppRoles.Administrador);

            if (isAdmin)
            {
                // Si es administrador, devuelve todas las propiedades
                var allProperties = await _propertyRepository.GetAllAsync();
                return Ok(allProperties.Select(x => x.ToPropertyRequest()));
            }

            // Para habitantes, obtener solo sus propiedades asignadas
            var propertyOwners = await _propertyOwnerRepository.GetByUserIdAsync(currentUserId, includeFinalized: false);
            var properties = propertyOwners.Select(po => po.Property.ToPropertyRequest());

            return Ok(properties);
        }

        /// <summary>
        /// Obtiene una propiedad por ID
        /// - Administradores: pueden ver cualquier propiedad
        /// - Habitantes: solo pueden ver sus propiedades asignadas
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
        public async Task<ActionResult<PropertyRequest>> GetById(int id)
        {
            Log.Information("GET > Property > Byid > User: {0}, Id: {1}", this.User.Identity.Name, id);

            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
                return NotFound();

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole(AppRoles.Administrador);

            // Si no es administrador, verificar que la propiedad esté asignada al usuario
            if (!isAdmin)
            {
                var userPropertyOwnership = await _propertyOwnerRepository.GetByPropertyAndUserIdAsync(id, currentUserId);
                if (userPropertyOwnership == null || userPropertyOwnership.EndDate != null)
                    return Forbid("No tiene permisos para acceder a esta propiedad");
            }

            return Ok(property.ToPropertyRequest());
        }

        /// <summary>
        /// Crea una nueva propiedad (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<ActionResult<PropertyRequest>> Create(PropertyRequest property)
        {
            Log.Information("POST > Property > User: {0}", this.User.Identity.Name);
            var propertyEntity = property.ToProperty();
            await _propertyRepository.AddAsync(propertyEntity);
            return CreatedAtAction(nameof(GetById), new { id = propertyEntity.Id }, propertyEntity.ToPropertyRequest());
        }

        /// <summary>
        /// Actualiza una propiedad existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Update(int id, PropertyRequest property)
        {
            Log.Information("PUT > Property > User: {0}", this.User.Identity.Name);
            if (id != property.Id)
                return BadRequest();

            // Verificar que la propiedad existe
            var existingProperty = await _propertyRepository.GetByIdAsync(id);
            if (existingProperty == null)
                return NotFound();

            existingProperty.LegalId = property.LegalId;
            existingProperty.Tower = property.Tower;
            existingProperty.Floor = property.Floor;
            existingProperty.Code = property.Code;

            await _propertyRepository.UpdateAsync(existingProperty);
            return Ok();
        }

        /// <summary>
        /// Elimina una propiedad (Soft Delete) (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Delete(int id)
        {
            Log.Information("DELETE > Property > User: {0}", this.User.Identity.Name);
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
                return NotFound();

            property.EndDate = DateTime.Now;
            await _propertyRepository.UpdateAsync(property);
            return Ok();
        }
    }
}
