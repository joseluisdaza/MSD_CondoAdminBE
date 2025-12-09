using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Condominio.Utils.Authorization;
using Condominio.Utils.Enums;
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
        /// Procesa el pago de un gasto pendiente (Solo Administradores)
        /// Valida que el gasto esté pendiente (statusId = 1), crea un pago automáticamente,
        /// crea la relación gasto-pago y actualiza el estado del gasto a pagado (statusId = 2)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ExpensePaymentResponse>> Create([FromBody] CreateExpensePaymentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > ExpensePayments > Create. User: {0}, ExpenseId: {1}", 
                    User.Identity?.Name, request.ExpenseId);

                // 1. Validar que el gasto existe
                var expense = await _expenseRepository.GetByIdAsync(request.ExpenseId);
                if (expense == null)
                {
                    return BadRequest("El gasto especificado no existe");
                }

                // 2. Validar que el gasto está pendiente (statusId = 1)
                if (expense.StatusId != 1)
                {
                    var statusMessage = expense.StatusId switch
                    {
                        2 => "El gasto ya está pagado.",
                        3 => "El gasto está verificado por administración.",
                        4 => "El gasto está cancelado",
                        0 => "El gasto tiene estado indefinido",
                        _ => $"El gasto tiene un estado no válido para pago (StatusId: {expense.StatusId})"
                    };
                    
                    Log.Warning("Payment creation failed - Invalid status. ExpenseId: {0}, StatusId: {1}", 
                        request.ExpenseId, expense.StatusId);
                    return BadRequest($"No se puede procesar el pago. {statusMessage}");
                }

                // 3. Verificar que no existe ya una relación de pago para este gasto
                var existingPayments = await _expensePaymentRepository.GetByExpenseIdAsync(request.ExpenseId);
                if (existingPayments.Any())
                {
                    return BadRequest("El gasto ya tiene un pago asociado");
                }

                // 4. Calcular el monto total del pago
                decimal paymentAmount;
                if (expense.InterestRate == null)
                {
                    // Si InterestRate es null, sumar amount + interestAmount
                    paymentAmount = expense.Amount + (expense.InterestAmount ?? 0);
                }
                else
                {
                    // Si InterestRate tiene valor, calcular: amount * (1 + InterestRate/100)
                    paymentAmount = expense.Amount * (1 + expense.InterestRate.Value / 100);
                }

                // 5. Crear el pago automáticamente
                var payment = new Condominio.Models.Payment
                {
                    ReceiveNumber = $"Expensa-{expense.Id}",  // Texto igual al ID del gasto
                    PaymentDate = DateTime.Now,
                    Amount = paymentAmount,
                    Description = $"Pago automático para gasto: {expense.Description}",
                    ReceivePhoto = "AUTO_PAYMENT" // Valor por defecto para pago automático
                };

                await _paymentRepository.AddAsync(payment);
                
                Log.Information("Automatic payment created. PaymentId: {0}, Amount: {1}, ExpenseId: {2}", 
                    payment.Id, paymentAmount, request.ExpenseId);

                // 6. Crear la relación gasto-pago
                var expensePayment = new Condominio.Models.ExpensePayment
                {
                    ExpenseId = request.ExpenseId,
                    PaymentId = payment.Id
                };

                await _expensePaymentRepository.AddAsync(expensePayment);
                
                Log.Information("ExpensePayment relation created. Id: {0}, ExpenseId: {1}, PaymentId: {2}", 
                    expensePayment.Id, request.ExpenseId, payment.Id);

                // 7. Actualizar el estado del gasto a pagado (statusId = 2)
                expense.StatusId = (int)PaymentStatus.Paid;
                await _expenseRepository.UpdateAsync(expense);
                
                Log.Information("Expense status updated to Paid. ExpenseId: {0}, Old Status: 1, New Status: 2", 
                    request.ExpenseId);

                // 8. Devolver la relación creada con todos los datos
                var createdExpensePayment = await _expensePaymentRepository.GetByIdAsync(expensePayment.Id);
                
                Log.Information("Expense payment process completed successfully. ExpenseId: {0}, PaymentId: {1}, Amount: {2}", 
                    request.ExpenseId, payment.Id, paymentAmount);

                return CreatedAtAction(nameof(GetById), new { id = expensePayment.Id }, 
                    createdExpensePayment?.ToExpensePaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing expense payment for ExpenseId: {0}", request.ExpenseId);
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