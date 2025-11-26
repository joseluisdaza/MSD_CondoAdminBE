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
    [Authorize]
    public class ServiceTypesController : ControllerBase
    {
        private readonly IServiceTypeRepository _serviceTypeRepository;

        public ServiceTypesController(IServiceTypeRepository serviceTypeRepository)
        {
            _serviceTypeRepository = serviceTypeRepository;
        }

        /// <summary>
        /// Obtiene todos los tipos de servicio (Solo Administradores y Directores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServiceTypeResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > ServiceTypes > GetAll. User: {0}", User.Identity?.Name);
                
                var serviceTypes = await _serviceTypeRepository.GetAllAsync();
                var serviceTypeResponses = serviceTypes.Select(st => st.ToServiceTypeResponse()).ToList();
                
                return Ok(serviceTypeResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all service types");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un tipo de servicio por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceTypeResponse>> GetById(int id)
        {
            try
            {
                Log.Information("GET > ServiceTypes > ById. User: {0}, Id: {1}", User.Identity?.Name, id);
                
                var serviceType = await _serviceTypeRepository.GetByIdAsync(id);
                if (serviceType == null)
                {
                    return NotFound($"Tipo de servicio con ID {id} no encontrado");
                }

                return Ok(serviceType.ToServiceTypeResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service type by ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un tipo de servicio por nombre
        /// </summary>
        [HttpGet("name/{serviceName}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceTypeResponse>> GetByServiceName(string serviceName)
        {
            try
            {
                Log.Information("GET > ServiceTypes > ByServiceName. User: {0}, ServiceName: {1}", User.Identity?.Name, serviceName);
                
                var serviceType = await _serviceTypeRepository.GetByServiceNameAsync(serviceName);
                if (serviceType == null)
                {
                    return NotFound($"Tipo de servicio con nombre '{serviceName}' no encontrado");
                }

                return Ok(serviceType.ToServiceTypeResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service type by name: {0}", serviceName);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de servicio (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceTypeResponse>> Create([FromBody] ServiceTypeRequest serviceTypeRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > ServiceTypes > Create. User: {0}, ServiceType: {@ServiceType}", 
                    User.Identity?.Name, serviceTypeRequest);

                // Validar que no existe otro tipo de servicio con el mismo nombre
                var existingServiceType = await _serviceTypeRepository.GetByServiceNameAsync(serviceTypeRequest.ServiceName);
                if (existingServiceType != null)
                {
                    return BadRequest($"Ya existe un tipo de servicio con el nombre '{serviceTypeRequest.ServiceName}'");
                }

                var serviceType = serviceTypeRequest.ToServiceType();
                await _serviceTypeRepository.AddAsync(serviceType);
                
                var createdServiceType = await _serviceTypeRepository.GetByIdAsync(serviceType.Id);
                return CreatedAtAction(nameof(GetById), new { id = serviceType.Id }, 
                    createdServiceType?.ToServiceTypeResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating service type");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un tipo de servicio existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceTypeResponse>> Update(int id, [FromBody] ServiceTypeRequest serviceTypeRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("PUT > ServiceTypes > Update. User: {0}, Id: {1}, ServiceType: {@ServiceType}", 
                    User.Identity?.Name, id, serviceTypeRequest);

                var existingServiceType = await _serviceTypeRepository.GetByIdAsync(id);
                if (existingServiceType == null)
                {
                    return NotFound($"Tipo de servicio con ID {id} no encontrado");
                }

                // Validar que no existe otro tipo de servicio con el mismo nombre (excepto el actual)
                var duplicateServiceType = await _serviceTypeRepository.GetByServiceNameAsync(serviceTypeRequest.ServiceName);
                if (duplicateServiceType != null && duplicateServiceType.Id != id)
                {
                    return BadRequest($"Ya existe otro tipo de servicio con el nombre '{serviceTypeRequest.ServiceName}'");
                }

                // Actualizar las propiedades
                existingServiceType.ServiceName = serviceTypeRequest.ServiceName;
                existingServiceType.Description = serviceTypeRequest.Description;

                await _serviceTypeRepository.UpdateAsync(existingServiceType);

                var updatedServiceType = await _serviceTypeRepository.GetByIdAsync(id);
                return Ok(updatedServiceType?.ToServiceTypeResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating service type with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un tipo de servicio (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Log.Information("DELETE > ServiceTypes > Delete. User: {0}, Id: {1}", User.Identity?.Name, id);

                var existingServiceType = await _serviceTypeRepository.GetByIdAsync(id);
                if (existingServiceType == null)
                {
                    return NotFound($"Tipo de servicio con ID {id} no encontrado");
                }

                // Verificar que no tenga gastos de servicio relacionados
                if (existingServiceType.ServiceExpenses?.Any() == true)
                {
                    return BadRequest("No se puede eliminar el tipo de servicio porque tiene gastos de servicio relacionados.");
                }

                await _serviceTypeRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting service type with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}