using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
            try
            {
                Serilog.Log.Information("POST > PropertyType > Create. User: {0}, Type: {1}", 
                    User.Identity?.Name, propertyType.Type);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar que no existe otro tipo de propiedad con el mismo Type
                var existingPropertyType = await _propertyTypeRepository.GetByTypeAsync(propertyType.Type);
                if (existingPropertyType != null)
                {
                    Serilog.Log.Warning("PropertyType creation failed - Type already exists: {0}", propertyType.Type);
                    return BadRequest($"Ya existe un tipo de propiedad con el nombre '{propertyType.Type}'");
                }

                var propertyTypeEntity = propertyType.ToPropertyType();
                await _propertyTypeRepository.AddAsync(propertyTypeEntity);
                
                Serilog.Log.Information("PropertyType created successfully. ID: {0}, Type: {1}", 
                    propertyTypeEntity.Id, propertyTypeEntity.Type);
                
                return CreatedAtAction(nameof(GetById), new { id = propertyTypeEntity.Id }, 
                    propertyTypeEntity.ToPropertyTypeRequest());
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error creating property type: {0}", propertyType.Type);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un tipo de propiedad existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Update(int id, PropertyTypeRequest propertyType)
        {
            try
            {
                Serilog.Log.Information("PUT > PropertyType > Update. User: {0}, ID: {1}, Type: {2}", 
                    User.Identity?.Name, id, propertyType.Type);

                if (id != propertyType.Id)
                {
                    return BadRequest("El ID del parámetro no coincide con el ID del objeto");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar que el tipo de propiedad existe
                var existingPropertyType = await _propertyTypeRepository.GetByIdAsync(id);
                if (existingPropertyType == null)
                {
                    return NotFound($"Tipo de propiedad con ID {id} no encontrado");
                }

                // Validar que no existe otro tipo de propiedad con el mismo Type (excepto el actual)
                var duplicatePropertyType = await _propertyTypeRepository.GetByTypeAsync(propertyType.Type);
                if (duplicatePropertyType != null && duplicatePropertyType.Id != id)
                {
                    Serilog.Log.Warning("PropertyType update failed - Type already exists: {0} (current ID: {1}, existing ID: {2})", 
                        propertyType.Type, id, duplicatePropertyType.Id);
                    return BadRequest($"Ya existe otro tipo de propiedad con el nombre '{propertyType.Type}'");
                }

                await _propertyTypeRepository.UpdateAsync(propertyType.ToPropertyType());
                
                Serilog.Log.Information("PropertyType updated successfully. ID: {0}, Type: {1}", id, propertyType.Type);
                
                return Ok(new { message = "Tipo de propiedad actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error updating property type ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
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
