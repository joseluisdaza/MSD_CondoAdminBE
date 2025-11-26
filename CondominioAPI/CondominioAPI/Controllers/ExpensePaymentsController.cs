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
    public class ExpensePaymentsController : ControllerBase
    {
        private readonly IExpensePaymentRepository _expensePaymentRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IPaymentRepository _paymentRepository;

        public ExpensePaymentsController(
            IExpensePaymentRepository expensePaymentRepository,
            IExpenseRepository expenseRepository,
            IPaymentRepository paymentRepository)
        {
            _expensePaymentRepository = expensePaymentRepository;
            _expenseRepository = expenseRepository;
            _paymentRepository = paymentRepository;
        }

        /// <summary>
        /// Obtiene todas las relaciones gasto-pago (Solo Administradores y Directores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ExpensePaymentResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > ExpensePayments > GetAll. User: {0}", User.Identity?.Name);
                
                var expensePayments = await _expensePaymentRepository.GetAllAsync();
                var expensePaymentResponses = expensePayments.Select(ep => ep.ToExpensePaymentResponse()).ToList();
                
                return Ok(expensePaymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all expense payments");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una relación gasto-pago por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<ExpensePaymentResponse>> GetById(int id)
        {
            try
            {
                Log.Information("GET > ExpensePayments > ById. User: {0}, Id: {1}", User.Identity?.Name, id);
                
                var expensePayment = await _expensePaymentRepository.GetByIdAsync(id);
                if (expensePayment == null)
                {
                    return NotFound($"Relación gasto-pago con ID {id} no encontrada");
                }

                return Ok(expensePayment.ToExpensePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting expense payment by ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene pagos por gasto
        /// </summary>
        [HttpGet("expense/{expenseId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ExpensePaymentResponse>>> GetByExpenseId(int expenseId)
        {
            try
            {
                Log.Information("GET > ExpensePayments > ByExpenseId. User: {0}, ExpenseId: {1}", User.Identity?.Name, expenseId);
                
                var expensePayments = await _expensePaymentRepository.GetByExpenseIdAsync(expenseId);
                var expensePaymentResponses = expensePayments.Select(ep => ep.ToExpensePaymentResponse()).ToList();
                
                return Ok(expensePaymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting expense payments by expense ID: {0}", expenseId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene gastos por pago
        /// </summary>
        [HttpGet("payment/{paymentId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ExpensePaymentResponse>>> GetByPaymentId(int paymentId)
        {
            try
            {
                Log.Information("GET > ExpensePayments > ByPaymentId. User: {0}, PaymentId: {1}", User.Identity?.Name, paymentId);
                
                var expensePayments = await _expensePaymentRepository.GetByPaymentIdAsync(paymentId);
                var expensePaymentResponses = expensePayments.Select(ep => ep.ToExpensePaymentResponse()).ToList();
                
                return Ok(expensePaymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting expense payments by payment ID: {0}", paymentId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una relación específica por gasto y pago
        /// </summary>
        [HttpGet("expense/{expenseId}/payment/{paymentId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<ExpensePaymentResponse>> GetByExpenseAndPaymentId(int expenseId, int paymentId)
        {
            try
            {
                Log.Information("GET > ExpensePayments > ByExpenseAndPaymentId. User: {0}, ExpenseId: {1}, PaymentId: {2}", 
                    User.Identity?.Name, expenseId, paymentId);
                
                var expensePayment = await _expensePaymentRepository.GetByExpenseAndPaymentIdAsync(expenseId, paymentId);
                if (expensePayment == null)
                {
                    return NotFound($"Relación entre gasto {expenseId} y pago {paymentId} no encontrada");
                }

                return Ok(expensePayment.ToExpensePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting expense payment by expense ID: {0} and payment ID: {1}", expenseId, paymentId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva relación gasto-pago (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ExpensePaymentResponse>> Create([FromBody] ExpensePaymentRequest expensePaymentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > ExpensePayments > Create. User: {0}, ExpensePayment: {@ExpensePayment}", 
                    User.Identity?.Name, expensePaymentRequest);

                // Validar que el gasto existe
                var expense = await _expenseRepository.GetByIdAsync(expensePaymentRequest.ExpenseId);
                if (expense == null)
                {
                    return BadRequest("El gasto especificado no existe");
                }

                // Validar que el pago existe
                var payment = await _paymentRepository.GetByIdAsync(expensePaymentRequest.PaymentId);
                if (payment == null)
                {
                    return BadRequest("El pago especificado no existe");
                }

                // Validar que la relación no existe ya
                var existingRelation = await _expensePaymentRepository.GetByExpenseAndPaymentIdAsync(
                    expensePaymentRequest.ExpenseId, expensePaymentRequest.PaymentId);
                if (existingRelation != null)
                {
                    return BadRequest("La relación entre el gasto y el pago ya existe");
                }

                var expensePayment = expensePaymentRequest.ToExpensePayment();
                await _expensePaymentRepository.AddAsync(expensePayment);
                
                var createdExpensePayment = await _expensePaymentRepository.GetByIdAsync(expensePayment.Id);
                return CreatedAtAction(nameof(GetById), new { id = expensePayment.Id }, 
                    createdExpensePayment?.ToExpensePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating expense payment");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una relación gasto-pago existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ExpensePaymentResponse>> Update(int id, [FromBody] ExpensePaymentRequest expensePaymentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("PUT > ExpensePayments > Update. User: {0}, Id: {1}, ExpensePayment: {@ExpensePayment}", 
                    User.Identity?.Name, id, expensePaymentRequest);

                var existingExpensePayment = await _expensePaymentRepository.GetByIdAsync(id);
                if (existingExpensePayment == null)
                {
                    return NotFound($"Relación gasto-pago con ID {id} no encontrada");
                }

                // Validar que el gasto existe
                var expense = await _expenseRepository.GetByIdAsync(expensePaymentRequest.ExpenseId);
                if (expense == null)
                {
                    return BadRequest("El gasto especificado no existe");
                }

                // Validar que el pago existe
                var payment = await _paymentRepository.GetByIdAsync(expensePaymentRequest.PaymentId);
                if (payment == null)
                {
                    return BadRequest("El pago especificado no existe");
                }

                // Validar que no existe otra relación con los mismos IDs (excepto la actual)
                var duplicateRelation = await _expensePaymentRepository.GetByExpenseAndPaymentIdAsync(
                    expensePaymentRequest.ExpenseId, expensePaymentRequest.PaymentId);
                if (duplicateRelation != null && duplicateRelation.Id != id)
                {
                    return BadRequest("Ya existe otra relación con el mismo gasto y pago");
                }

                // Actualizar las propiedades
                existingExpensePayment.ExpenseId = expensePaymentRequest.ExpenseId;
                existingExpensePayment.PaymentId = expensePaymentRequest.PaymentId;

                await _expensePaymentRepository.UpdateAsync(existingExpensePayment);

                var updatedExpensePayment = await _expensePaymentRepository.GetByIdAsync(id);
                return Ok(updatedExpensePayment?.ToExpensePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating expense payment with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una relación gasto-pago (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Log.Information("DELETE > ExpensePayments > Delete. User: {0}, Id: {1}", User.Identity?.Name, id);

                var existingExpensePayment = await _expensePaymentRepository.GetByIdAsync(id);
                if (existingExpensePayment == null)
                {
                    return NotFound($"Relación gasto-pago con ID {id} no encontrada");
                }

                await _expensePaymentRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting expense payment with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una relación específica por gasto y pago (Solo Administradores)
        /// </summary>
        [HttpDelete("expense/{expenseId}/payment/{paymentId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> DeleteByExpenseAndPaymentId(int expenseId, int paymentId)
        {
            try
            {
                Log.Information("DELETE > ExpensePayments > DeleteByExpenseAndPaymentId. User: {0}, ExpenseId: {1}, PaymentId: {2}", 
                    User.Identity?.Name, expenseId, paymentId);

                var expensePayment = await _expensePaymentRepository.GetByExpenseAndPaymentIdAsync(expenseId, paymentId);
                if (expensePayment == null)
                {
                    return NotFound($"Relación entre gasto {expenseId} y pago {paymentId} no encontrada");
                }

                await _expensePaymentRepository.DeleteAsync(expensePayment.Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting expense payment by expense ID: {0} and payment ID: {1}", expenseId, paymentId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}