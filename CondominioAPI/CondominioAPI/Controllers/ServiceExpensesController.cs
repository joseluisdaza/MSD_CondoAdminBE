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
    public class ServiceExpensesController : ControllerBase
    {
        private readonly IServiceExpenseRepository _serviceExpenseRepository;
        private readonly IServiceTypeRepository _serviceTypeRepository;
        private readonly IPaymentStatusRepository _paymentStatusRepository;

        public ServiceExpensesController(
            IServiceExpenseRepository serviceExpenseRepository,
            IServiceTypeRepository serviceTypeRepository,
            IPaymentStatusRepository paymentStatusRepository)
        {
            _serviceExpenseRepository = serviceExpenseRepository;
            _serviceTypeRepository = serviceTypeRepository;
            _paymentStatusRepository = paymentStatusRepository;
        }

        /// <summary>
        /// Obtiene todos los gastos de servicio (Solo Administradores y Directores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServiceExpenseResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > ServiceExpenses > GetAll. User: {0}", User.Identity?.Name);
                
                var serviceExpenses = await _serviceExpenseRepository.GetAllAsync();
                var serviceExpenseResponses = serviceExpenses.Select(se => se.ToServiceExpenseResponse()).ToList();
                
                return Ok(serviceExpenseResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all service expenses");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un gasto de servicio por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceExpenseResponse>> GetById(int id)
        {
            try
            {
                Log.Information("GET > ServiceExpenses > ById. User: {0}, Id: {1}", User.Identity?.Name, id);
                
                var serviceExpense = await _serviceExpenseRepository.GetByIdAsync(id);
                if (serviceExpense == null)
                {
                    return NotFound($"Gasto de servicio con ID {id} no encontrado");
                }

                return Ok(serviceExpense.ToServiceExpenseResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service expense by ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene gastos de servicio por tipo de servicio
        /// </summary>
        [HttpGet("service-type/{serviceTypeId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServiceExpenseResponse>>> GetByServiceTypeId(int serviceTypeId)
        {
            try
            {
                Log.Information("GET > ServiceExpenses > ByServiceTypeId. User: {0}, ServiceTypeId: {1}", 
                    User.Identity?.Name, serviceTypeId);
                
                var serviceExpenses = await _serviceExpenseRepository.GetByServiceTypeIdAsync(serviceTypeId);
                var serviceExpenseResponses = serviceExpenses.Select(se => se.ToServiceExpenseResponse()).ToList();
                
                return Ok(serviceExpenseResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service expenses by service type ID: {0}", serviceTypeId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene gastos de servicio por estado
        /// </summary>
        [HttpGet("status/{statusId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServiceExpenseResponse>>> GetByStatusId(int statusId)
        {
            try
            {
                Log.Information("GET > ServiceExpenses > ByStatusId. User: {0}, StatusId: {1}", 
                    User.Identity?.Name, statusId);
                
                var serviceExpenses = await _serviceExpenseRepository.GetByStatusIdAsync(statusId);
                var serviceExpenseResponses = serviceExpenses.Select(se => se.ToServiceExpenseResponse()).ToList();
                
                return Ok(serviceExpenseResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting service expenses by status ID: {0}", statusId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene gastos de servicio pendientes
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServiceExpenseResponse>>> GetPendingExpenses()
        {
            try
            {
                Log.Information("GET > ServiceExpenses > Pending. User: {0}", User.Identity?.Name);
                
                var serviceExpenses = await _serviceExpenseRepository.GetPendingExpensesAsync();
                var serviceExpenseResponses = serviceExpenses.Select(se => se.ToServiceExpenseResponse()).ToList();
                
                return Ok(serviceExpenseResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting pending service expenses");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene gastos de servicio vencidos
        /// </summary>
        [HttpGet("overdue")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ServiceExpenseResponse>>> GetOverdueExpenses()
        {
            try
            {
                Log.Information("GET > ServiceExpenses > Overdue. User: {0}", User.Identity?.Name);
                
                var serviceExpenses = await _serviceExpenseRepository.GetOverdueExpensesAsync();
                var serviceExpenseResponses = serviceExpenses.Select(se => se.ToServiceExpenseResponse()).ToList();
                
                return Ok(serviceExpenseResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting overdue service expenses");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo gasto de servicio (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceExpenseResponse>> Create([FromBody] ServiceExpenseRequest serviceExpenseRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > ServiceExpenses > Create. User: {0}, ServiceExpense: {@ServiceExpense}", 
                    User.Identity?.Name, serviceExpenseRequest);

                // Validar que el tipo de servicio existe
                var serviceType = await _serviceTypeRepository.GetByIdAsync(serviceExpenseRequest.ServiceTypeId);
                if (serviceType == null)
                {
                    return BadRequest("El tipo de servicio especificado no existe");
                }

                // Validar que el estado existe
                var status = await _paymentStatusRepository.GetByIdAsync(serviceExpenseRequest.StatusId);
                if (status == null)
                {
                    return BadRequest("El estado especificado no existe");
                }

                // Validar fechas
                if (serviceExpenseRequest.PaymentLimitDate <= serviceExpenseRequest.StartDate)
                {
                    return BadRequest("La fecha límite de pago debe ser posterior a la fecha de inicio");
                }

                var serviceExpense = serviceExpenseRequest.ToServiceExpense();
                await _serviceExpenseRepository.AddAsync(serviceExpense);
                
                var createdServiceExpense = await _serviceExpenseRepository.GetByIdAsync(serviceExpense.Id);
                return CreatedAtAction(nameof(GetById), new { id = serviceExpense.Id }, 
                    createdServiceExpense?.ToServiceExpenseResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating service expense");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un gasto de servicio existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ServiceExpenseResponse>> Update(int id, [FromBody] ServiceExpenseRequest serviceExpenseRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("PUT > ServiceExpenses > Update. User: {0}, Id: {1}, ServiceExpense: {@ServiceExpense}", 
                    User.Identity?.Name, id, serviceExpenseRequest);

                var existingServiceExpense = await _serviceExpenseRepository.GetByIdAsync(id);
                if (existingServiceExpense == null)
                {
                    return NotFound($"Gasto de servicio con ID {id} no encontrado");
                }

                // Validar que el tipo de servicio existe
                var serviceType = await _serviceTypeRepository.GetByIdAsync(serviceExpenseRequest.ServiceTypeId);
                if (serviceType == null)
                {
                    return BadRequest("El tipo de servicio especificado no existe");
                }

                // Validar que el estado existe
                var status = await _paymentStatusRepository.GetByIdAsync(serviceExpenseRequest.StatusId);
                if (status == null)
                {
                    return BadRequest("El estado especificado no existe");
                }

                // Validar fechas
                if (serviceExpenseRequest.PaymentLimitDate <= serviceExpenseRequest.StartDate)
                {
                    return BadRequest("La fecha límite de pago debe ser posterior a la fecha de inicio");
                }

                // Actualizar las propiedades
                existingServiceExpense.ServiceTypeId = serviceExpenseRequest.ServiceTypeId;
                existingServiceExpense.Description = serviceExpenseRequest.Description;
                existingServiceExpense.Amount = serviceExpenseRequest.Amount;
                existingServiceExpense.StartDate = serviceExpenseRequest.StartDate;
                existingServiceExpense.PaymentLimitDate = serviceExpenseRequest.PaymentLimitDate;
                existingServiceExpense.InterestAmount = serviceExpenseRequest.InterestAmount;
                existingServiceExpense.TotalAmount = serviceExpenseRequest.TotalAmount;
                existingServiceExpense.Status = serviceExpenseRequest.Status;
                existingServiceExpense.ExpenseDate = serviceExpenseRequest.ExpenseDate;
                existingServiceExpense.StatusId = serviceExpenseRequest.StatusId;

                await _serviceExpenseRepository.UpdateAsync(existingServiceExpense);

                var updatedServiceExpense = await _serviceExpenseRepository.GetByIdAsync(id);
                return Ok(updatedServiceExpense?.ToServiceExpenseResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating service expense with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un gasto de servicio (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Log.Information("DELETE > ServiceExpenses > Delete. User: {0}, Id: {1}", User.Identity?.Name, id);

                var existingServiceExpense = await _serviceExpenseRepository.GetByIdAsync(id);
                if (existingServiceExpense == null)
                {
                    return NotFound($"Gasto de servicio con ID {id} no encontrado");
                }

                // Verificar que no tenga pagos relacionados
                if (existingServiceExpense.ServiceExpensePayments?.Any() == true)
                {
                    return BadRequest("No se puede eliminar el gasto de servicio porque tiene pagos relacionados. " +
                                      "Elimine primero las relaciones de pago.");
                }

                await _serviceExpenseRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting service expense with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}