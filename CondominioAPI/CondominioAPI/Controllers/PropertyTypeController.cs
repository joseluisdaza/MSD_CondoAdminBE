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
    public class PropertyTypeController : ControllerBase
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public PropertyTypeController(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        /// <summary>
        /// Obtiene todos los tipos de propiedad (Administrador y Habitante pueden ver)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<PropertyTypeRequest>>> GetAll()
        {
            var propertyTypes = await _propertyTypeRepository.GetAllAsync();
            return Ok(propertyTypes.Select(x => x.ToPropertyTypeRequest()));
        }

        /// <summary>
        /// Obtiene un tipo de propiedad por ID (Administrador y Habitante pueden ver)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<PropertyTypeRequest>> GetById(int id)
        {
            var propertyType = await _propertyTypeRepository.GetByIdAsync(id);
            if (propertyType == null)
                return NotFound();
            return Ok(propertyType.ToPropertyTypeRequest());
        }

        /// <summary>
        /// Crea un nuevo tipo de propiedad (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<ActionResult<PropertyTypeRequest>> Create(PropertyTypeRequest propertyType)
        {
            var propertyTypeEntity = propertyType.ToPropertyType();
            await _propertyTypeRepository.AddAsync(propertyTypeEntity);
            return CreatedAtAction(nameof(GetById), new { id = propertyTypeEntity.Id }, propertyTypeEntity.ToPropertyTypeRequest());
        }

        /// <summary>
        /// Actualiza un tipo de propiedad existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Update(int id, PropertyTypeRequest propertyType)
        {
            if (id != propertyType.Id)
                return BadRequest();

            await _propertyTypeRepository.UpdateAsync(propertyType.ToPropertyType());
            return Ok();
        }

        /// <summary>
        /// Elimina un tipo de propiedad (Soft Delete) (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Delete(int id)
        {
            var propertyType = await _propertyTypeRepository.GetByIdAsync(id);
            if (propertyType == null)
                return NotFound();

            propertyType.EndDate = DateTime.Now;
            await _propertyTypeRepository.UpdateAsync(propertyType);
            return Ok();
        }
    }
}
