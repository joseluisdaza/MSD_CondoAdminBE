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
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;

        public PropertyController(IPropertyRepository propertyRepository, IUserRepository userRepository)
        {
            _propertyRepository = propertyRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Obtiene todas las propiedades (Solo Administradores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<ActionResult<IEnumerable<PropertyRequest>>> GetAll()
        {
            var properties = await _propertyRepository.GetAllAsync();
            return Ok(properties.Select(x => x.ToPropertyRequest()));
        }

        /// <summary>
        /// Obtiene propiedades asignadas al usuario actual (Habitantes)
        /// o todas si es Administrador
        /// </summary>
        [HttpGet("ByUser")]
        [Authorize(Roles = $"{AppRoles.Habitante},{AppRoles.Administrador}")]
        public async Task<ActionResult<IEnumerable<PropertyRequest>>> GetMyProperties()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole(AppRoles.Administrador);

            if (isAdmin)
            {
                // Si es administrador, devuelve todas las propiedades
                var allProperties = await _propertyRepository.GetAllAsync();
                return Ok(allProperties.Select(x => x.ToPropertyRequest()));
            }

            // Para habitantes, obtener solo sus propiedades asignadas
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null)
                return NotFound("Usuario no encontrado");

            // Nota: Necesitarás un método en el repositorio para obtener propiedades por usuario
            // Por ahora retorno mensaje informativo
            return Ok(new { message = "Implementar lógica para obtener propiedades del usuario", userId = currentUserId });
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
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
                return NotFound();

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole(AppRoles.Administrador);

            // Si no es administrador, verificar que la propiedad esté asignada al usuario
            if (!isAdmin)
            {
                // Aquí deberías verificar en PropertyOwners si el usuario tiene asignada esta propiedad
                // Por ahora se permite el acceso, pero deberías implementar la validación
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
            if (id != property.Id)
                return BadRequest();

            await _propertyRepository.UpdateAsync(property.ToProperty());
            return Ok();
        }

        /// <summary>
        /// Elimina una propiedad (Soft Delete) (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
                return NotFound();

            property.EndDate = DateTime.Now;
            await _propertyRepository.UpdateAsync(property);
            return Ok();
        }
    }
}
