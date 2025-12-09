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
    public class ServicePaymentsController : ControllerBase
    {
        private readonly IServicePaymentRepository _servicePaymentRepository;
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IServiceExpensePaymentRepository _serviceExpensePaymentRepository;
        private readonly IServiceExpenseRepository _serviceExpenseRepository;

        public ServicePaymentsController(
            IServicePaymentRepository servicePaymentRepository,
            IPaymentStatusRepository paymentStatusRepository,
            IServiceExpensePaymentRepository serviceExpensePaymentRepository,
            IServiceExpenseRepository serviceExpenseRepository)
        {
            _servicePaymentRepository = servicePaymentRepository;
            _paymentStatusRepository = paymentStatusRepository;
            _serviceExpensePaymentRepository = serviceExpensePaymentRepository;
            _serviceExpenseRepository = serviceExpenseRepository;
        }

        /// <summary>
        /// Obtiene todos los pagos de servicio (Solo Administradores y Directores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServicePaymentResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > ServicePayments > GetAll. User: {0}", User.Identity?.Name);
                
                var servicePayments = await _servicePaymentRepository.GetAllAsync();
                var servicePaymentResponses = servicePayments.Select(sp => sp.ToServicePaymentResponse()).ToList();
                
                return Ok(servicePaymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all service payments");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un pago de servicio por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<ServicePaymentResponse>> GetById(int id)
        {
            try
            {
                Log.Information("GET > ServicePayments > ById. User: {0}, Id: {1}", User.Identity?.Name, id);
                
                var servicePayment = await _servicePaymentRepository.GetByIdAsync(id);
                if (servicePayment == null)
                {
                    return NotFound($"Pago de servicio con ID {id} no encontrado");
                }

                return Ok(servicePayment.ToServicePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service payment by ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un pago de servicio por número de recibo
        /// </summary>
        [HttpGet("receive-number/{receiveNumber}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<ServicePaymentResponse>> GetByReceiveNumber(string receiveNumber)
        {
            try
            {
                Log.Information("GET > ServicePayments > ByReceiveNumber. User: {0}, ReceiveNumber: {1}", 
                    User.Identity?.Name, receiveNumber);
                
                var servicePayment = await _servicePaymentRepository.GetByReceiveNumberAsync(receiveNumber);
                if (servicePayment == null)
                {
                    return NotFound($"Pago de servicio con número de recibo {receiveNumber} no encontrado");
                }

                return Ok(servicePayment.ToServicePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service payment by receive number: {0}", receiveNumber);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene pagos de servicio por estado
        /// </summary>
        [HttpGet("status/{statusId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServicePaymentResponse>>> GetByStatusId(int statusId)
        {
            try
            {
                Log.Information("GET > ServicePayments > ByStatusId. User: {0}, StatusId: {1}", 
                    User.Identity?.Name, statusId);
                
                var servicePayments = await _servicePaymentRepository.GetByStatusIdAsync(statusId);
                var servicePaymentResponses = servicePayments.Select(sp => sp.ToServicePaymentResponse()).ToList();
                
                return Ok(servicePaymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service payments by status ID: {0}", statusId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene pagos de servicio por rango de fechas
        /// </summary>
        [HttpGet("date-range")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServicePaymentResponse>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                Log.Information("GET > ServicePayments > ByDateRange. User: {0}, StartDate: {1}, EndDate: {2}", 
                    User.Identity?.Name, startDate, endDate);
                
                if (startDate > endDate)
                {
                    return BadRequest("La fecha de inicio no puede ser mayor que la fecha de fin");
                }

                var servicePayments = await _servicePaymentRepository.GetByDateRangeAsync(startDate, endDate);
                var servicePaymentResponses = servicePayments.Select(sp => sp.ToServicePaymentResponse()).ToList();
                
                return Ok(servicePaymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service payments by date range: {0} to {1}", startDate, endDate);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo pago de servicio y la relación con el gasto de servicio (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ServicePaymentResponse>> Create([FromBody] ServicePaymentRequest servicePaymentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > ServicePayments > Create. User: {0}, ServicePayment: {@ServicePayment}", 
                    User.Identity?.Name, servicePaymentRequest);

                // Validar que no existe otro pago con el mismo número de recibo
                var existingPayment = await _servicePaymentRepository.GetByReceiveNumberAsync(servicePaymentRequest.ReceiveNumber);
                if (existingPayment != null)
                {
                    return BadRequest($"Ya existe un pago de servicio con el número de recibo {servicePaymentRequest.ReceiveNumber}");
                }

                // Validar que el estado existe
                var status = await _paymentStatusRepository.GetByIdAsync(servicePaymentRequest.StatusId);
                if (status == null)
                {
                    return BadRequest("El estado especificado no existe");
                }

                // Validar que el gasto de servicio existe
                var serviceExpense = await _serviceExpenseRepository.GetByIdAsync(servicePaymentRequest.ServiceExpenseId);
                if (serviceExpense == null)
                {
                    return BadRequest("El gasto de servicio especificado no existe");
                }

                // Verificar que no existe ya una relación para este gasto de servicio
                var existingRelations = await _serviceExpensePaymentRepository.GetByServiceExpenseIdAsync(servicePaymentRequest.ServiceExpenseId);
                if (existingRelations.Any())
                {
                    return BadRequest("El gasto de servicio ya tiene un pago asociado");
                }

                // Crear el pago de servicio
                var servicePayment = servicePaymentRequest.ToServicePayment();
                await _servicePaymentRepository.AddAsync(servicePayment);
                
                Log.Information("ServicePayment created successfully. Id: {0}, ReceiveNumber: {1}", 
                    servicePayment.Id, servicePayment.ReceiveNumber);

                // Crear la relación ServiceExpensePayment
                var serviceExpensePayment = new Condominio.Models.ServiceExpensePayment
                {
                    ServiceExpenseId = servicePaymentRequest.ServiceExpenseId,
                    PaymentId = servicePayment.Id
                };

                await _serviceExpensePaymentRepository.AddAsync(serviceExpensePayment);
                
                Log.Information("ServiceExpensePayment relation created successfully. ServiceExpenseId: {0}, PaymentId: {1}", 
                    servicePaymentRequest.ServiceExpenseId, servicePayment.Id);

                // Actualizar el estado del ServiceExpense a pagado (StatusId = 2)
                serviceExpense.StatusId = 2;
                await _serviceExpenseRepository.UpdateAsync(serviceExpense);
                
                Log.Information("ServiceExpense status updated to Paid. ServiceExpenseId: {0}, Old Status: {1}, New Status: 2", 
                    servicePaymentRequest.ServiceExpenseId, serviceExpense.StatusId);

                // Obtener el pago creado con todas las relaciones
                var createdServicePayment = await _servicePaymentRepository.GetByIdAsync(servicePayment.Id);
                
                Log.Information("Service payment process completed successfully. PaymentId: {0}, ServiceExpenseId: {1}, ServiceExpense Status updated to Paid", 
                    servicePayment.Id, servicePaymentRequest.ServiceExpenseId);

                return CreatedAtAction(nameof(GetById), new { id = servicePayment.Id }, 
                    createdServicePayment?.ToServicePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating service payment and relation. ServiceExpenseId: {0}", servicePaymentRequest?.ServiceExpenseId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un pago de servicio existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ServicePaymentResponse>> Update(int id, [FromBody] ServicePaymentRequest servicePaymentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("PUT > ServicePayments > Update. User: {0}, Id: {1}, ServicePayment: {@ServicePayment}", 
                    User.Identity?.Name, id, servicePaymentRequest);

                var existingServicePayment = await _servicePaymentRepository.GetByIdAsync(id);
                if (existingServicePayment == null)
                {
                    return NotFound($"Pago de servicio con ID {id} no encontrado");
                }

                // Validar que no existe otro pago con el mismo número de recibo (excepto el actual)
                var duplicatePayment = await _servicePaymentRepository.GetByReceiveNumberAsync(servicePaymentRequest.ReceiveNumber);
                if (duplicatePayment != null && duplicatePayment.Id != id)
                {
                    return BadRequest($"Ya existe otro pago de servicio con el número de recibo {servicePaymentRequest.ReceiveNumber}");
                }

                // Validar que el estado existe
                var status = await _paymentStatusRepository.GetByIdAsync(servicePaymentRequest.StatusId);
                if (status == null)
                {
                    return BadRequest("El estado especificado no existe");
                }

                // Actualizar las propiedades
                existingServicePayment.ReceiveNumber = servicePaymentRequest.ReceiveNumber;
                existingServicePayment.PaymentDate = servicePaymentRequest.PaymentDate;
                existingServicePayment.Amount = servicePaymentRequest.Amount;
                existingServicePayment.Description = servicePaymentRequest.Description;
                existingServicePayment.ReceivePhoto = servicePaymentRequest.ReceivePhoto;
                existingServicePayment.StatusId = servicePaymentRequest.StatusId;

                await _servicePaymentRepository.UpdateAsync(existingServicePayment);

                var updatedServicePayment = await _servicePaymentRepository.GetByIdAsync(id);
                return Ok(updatedServicePayment?.ToServicePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating service payment with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un pago de servicio (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Log.Information("DELETE > ServicePayments > Delete. User: {0}, Id: {1}", User.Identity?.Name, id);

                var existingServicePayment = await _servicePaymentRepository.GetByIdAsync(id);
                if (existingServicePayment == null)
                {
                    return NotFound($"Pago de servicio con ID {id} no encontrado");
                }

                // Verificar que no tenga relaciones con gastos de servicio
                if (existingServicePayment.ServiceExpensePayments?.Any() == true)
                {
                    return BadRequest("No se puede eliminar el pago de servicio porque tiene relaciones con gastos de servicio. " +
                                      "Elimine primero las relaciones gasto-pago.");
                }

                await _servicePaymentRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting service payment with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}