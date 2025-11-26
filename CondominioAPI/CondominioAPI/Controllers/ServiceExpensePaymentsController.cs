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
    public class ServiceExpensePaymentsController : ControllerBase
    {
        private readonly IServiceExpensePaymentRepository _serviceExpensePaymentRepository;
        private readonly IServiceExpenseRepository _serviceExpenseRepository;
        private readonly IServicePaymentRepository _servicePaymentRepository;

        public ServiceExpensePaymentsController(
            IServiceExpensePaymentRepository serviceExpensePaymentRepository,
            IServiceExpenseRepository serviceExpenseRepository,
            IServicePaymentRepository servicePaymentRepository)
        {
            _serviceExpensePaymentRepository = serviceExpensePaymentRepository;
            _serviceExpenseRepository = serviceExpenseRepository;
            _servicePaymentRepository = servicePaymentRepository;
        }

        /// <summary>
        /// Obtiene todas las relaciones gasto-pago de servicio (Solo Administradores y Directores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServiceExpensePaymentResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > ServiceExpensePayments > GetAll. User: {0}", User.Identity?.Name);
                
                var serviceExpensePayments = await _serviceExpensePaymentRepository.GetAllAsync();
                var serviceExpensePaymentResponses = serviceExpensePayments.Select(sep => sep.ToServiceExpensePaymentResponse()).ToList();
                
                return Ok(serviceExpensePaymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all service expense payments");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una relación gasto-pago de servicio por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceExpensePaymentResponse>> GetById(int id)
        {
            try
            {
                Log.Information("GET > ServiceExpensePayments > ById. User: {0}, Id: {1}", User.Identity?.Name, id);
                
                var serviceExpensePayment = await _serviceExpensePaymentRepository.GetByIdAsync(id);
                if (serviceExpensePayment == null)
                {
                    return NotFound($"Relación gasto-pago de servicio con ID {id} no encontrada");
                }

                return Ok(serviceExpensePayment.ToServiceExpensePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service expense payment by ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene pagos por gasto de servicio
        /// </summary>
        [HttpGet("service-expense/{serviceExpenseId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServiceExpensePaymentResponse>>> GetByServiceExpenseId(int serviceExpenseId)
        {
            try
            {
                Log.Information("GET > ServiceExpensePayments > ByServiceExpenseId. User: {0}, ServiceExpenseId: {1}", 
                    User.Identity?.Name, serviceExpenseId);
                
                var serviceExpensePayments = await _serviceExpensePaymentRepository.GetByServiceExpenseIdAsync(serviceExpenseId);
                var serviceExpensePaymentResponses = serviceExpensePayments.Select(sep => sep.ToServiceExpensePaymentResponse()).ToList();
                
                return Ok(serviceExpensePaymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service expense payments by service expense ID: {0}", serviceExpenseId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene gastos de servicio por pago
        /// </summary>
        [HttpGet("payment/{paymentId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServiceExpensePaymentResponse>>> GetByPaymentId(int paymentId)
        {
            try
            {
                Log.Information("GET > ServiceExpensePayments > ByPaymentId. User: {0}, PaymentId: {1}", 
                    User.Identity?.Name, paymentId);
                
                var serviceExpensePayments = await _serviceExpensePaymentRepository.GetByPaymentIdAsync(paymentId);
                var serviceExpensePaymentResponses = serviceExpensePayments.Select(sep => sep.ToServiceExpensePaymentResponse()).ToList();
                
                return Ok(serviceExpensePaymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service expense payments by payment ID: {0}", paymentId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una relación específica por gasto de servicio y pago
        /// </summary>
        [HttpGet("service-expense/{serviceExpenseId}/payment/{paymentId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceExpensePaymentResponse>> GetByServiceExpenseAndPaymentId(int serviceExpenseId, int paymentId)
        {
            try
            {
                Log.Information("GET > ServiceExpensePayments > ByServiceExpenseAndPaymentId. User: {0}, ServiceExpenseId: {1}, PaymentId: {2}", 
                    User.Identity?.Name, serviceExpenseId, paymentId);
                
                var serviceExpensePayment = await _serviceExpensePaymentRepository.GetByServiceExpenseAndPaymentIdAsync(serviceExpenseId, paymentId);
                if (serviceExpensePayment == null)
                {
                    return NotFound($"Relación entre gasto de servicio {serviceExpenseId} y pago {paymentId} no encontrada");
                }

                return Ok(serviceExpensePayment.ToServiceExpensePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service expense payment by service expense ID: {0} and payment ID: {1}", 
                    serviceExpenseId, paymentId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva relación gasto-pago de servicio (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceExpensePaymentResponse>> Create([FromBody] ServiceExpensePaymentRequest serviceExpensePaymentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > ServiceExpensePayments > Create. User: {0}, ServiceExpensePayment: {@ServiceExpensePayment}", 
                    User.Identity?.Name, serviceExpensePaymentRequest);

                // Validar que el gasto de servicio existe
                var serviceExpense = await _serviceExpenseRepository.GetByIdAsync(serviceExpensePaymentRequest.ServiceExpenseId);
                if (serviceExpense == null)
                {
                    return BadRequest("El gasto de servicio especificado no existe");
                }

                // Validar que el pago de servicio existe
                var servicePayment = await _servicePaymentRepository.GetByIdAsync(serviceExpensePaymentRequest.PaymentId);
                if (servicePayment == null)
                {
                    return BadRequest("El pago de servicio especificado no existe");
                }

                // Validar que la relación no existe ya
                var existingRelation = await _serviceExpensePaymentRepository.GetByServiceExpenseAndPaymentIdAsync(
                    serviceExpensePaymentRequest.ServiceExpenseId, serviceExpensePaymentRequest.PaymentId);
                if (existingRelation != null)
                {
                    return BadRequest("La relación entre el gasto de servicio y el pago ya existe");
                }

                var serviceExpensePayment = serviceExpensePaymentRequest.ToServiceExpensePayment();
                await _serviceExpensePaymentRepository.AddAsync(serviceExpensePayment);
                
                var createdServiceExpensePayment = await _serviceExpensePaymentRepository.GetByIdAsync(serviceExpensePayment.Id);
                return CreatedAtAction(nameof(GetById), new { id = serviceExpensePayment.Id }, 
                    createdServiceExpensePayment?.ToServiceExpensePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating service expense payment");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una relación gasto-pago de servicio existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceExpensePaymentResponse>> Update(int id, [FromBody] ServiceExpensePaymentRequest serviceExpensePaymentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("PUT > ServiceExpensePayments > Update. User: {0}, Id: {1}, ServiceExpensePayment: {@ServiceExpensePayment}", 
                    User.Identity?.Name, id, serviceExpensePaymentRequest);

                var existingServiceExpensePayment = await _serviceExpensePaymentRepository.GetByIdAsync(id);
                if (existingServiceExpensePayment == null)
                {
                    return NotFound($"Relación gasto-pago de servicio con ID {id} no encontrada");
                }

                // Validar que el gasto de servicio existe
                var serviceExpense = await _serviceExpenseRepository.GetByIdAsync(serviceExpensePaymentRequest.ServiceExpenseId);
                if (serviceExpense == null)
                {
                    return BadRequest("El gasto de servicio especificado no existe");
                }

                // Validar que el pago de servicio existe
                var servicePayment = await _servicePaymentRepository.GetByIdAsync(serviceExpensePaymentRequest.PaymentId);
                if (servicePayment == null)
                {
                    return BadRequest("El pago de servicio especificado no existe");
                }

                // Validar que no existe otra relación con los mismos IDs (excepto la actual)
                var duplicateRelation = await _serviceExpensePaymentRepository.GetByServiceExpenseAndPaymentIdAsync(
                    serviceExpensePaymentRequest.ServiceExpenseId, serviceExpensePaymentRequest.PaymentId);
                if (duplicateRelation != null && duplicateRelation.Id != id)
                {
                    return BadRequest("Ya existe otra relación con el mismo gasto de servicio y pago");
                }

                // Actualizar las propiedades
                existingServiceExpensePayment.ServiceExpenseId = serviceExpensePaymentRequest.ServiceExpenseId;
                existingServiceExpensePayment.PaymentId = serviceExpensePaymentRequest.PaymentId;

                await _serviceExpensePaymentRepository.UpdateAsync(existingServiceExpensePayment);

                var updatedServiceExpensePayment = await _serviceExpensePaymentRepository.GetByIdAsync(id);
                return Ok(updatedServiceExpensePayment?.ToServiceExpensePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating service expense payment with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una relación gasto-pago de servicio (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Log.Information("DELETE > ServiceExpensePayments > Delete. User: {0}, Id: {1}", User.Identity?.Name, id);

                var existingServiceExpensePayment = await _serviceExpensePaymentRepository.GetByIdAsync(id);
                if (existingServiceExpensePayment == null)
                {
                    return NotFound($"Relación gasto-pago de servicio con ID {id} no encontrada");
                }

                await _serviceExpensePaymentRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting service expense payment with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una relación específica por gasto de servicio y pago (Solo Administradores)
        /// </summary>
        [HttpDelete("service-expense/{serviceExpenseId}/payment/{paymentId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> DeleteByServiceExpenseAndPaymentId(int serviceExpenseId, int paymentId)
        {
            try
            {
                Log.Information("DELETE > ServiceExpensePayments > DeleteByServiceExpenseAndPaymentId. User: {0}, ServiceExpenseId: {1}, PaymentId: {2}", 
                    User.Identity?.Name, serviceExpenseId, paymentId);

                var serviceExpensePayment = await _serviceExpensePaymentRepository.GetByServiceExpenseAndPaymentIdAsync(serviceExpenseId, paymentId);
                if (serviceExpensePayment == null)
                {
                    return NotFound($"Relación entre gasto de servicio {serviceExpenseId} y pago {paymentId} no encontrada");
                }

                await _serviceExpensePaymentRepository.DeleteAsync(serviceExpensePayment.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting service expense payment by service expense ID: {0} and payment ID: {1}", 
                    serviceExpenseId, paymentId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}