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
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentsController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        /// <summary>
        /// Obtiene todos los pagos (Solo Administradores y Directores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<PaymentResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > Payments > GetAll. User: {0}", User.Identity?.Name);
                
                var payments = await _paymentRepository.GetAllWithRelationsAsync();
                var paymentResponses = payments.Select(p => p.ToPaymentResponse()).ToList();
                
                return Ok(paymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all payments");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un pago por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<PaymentResponse>> GetById(int id)
        {
            try
            {
                Log.Information("GET > Payments > ById. User: {0}, Id: {1}", User.Identity?.Name, id);
                
                var payment = await _paymentRepository.GetByIdWithRelationsAsync(id);
                if (payment == null)
                {
                    return NotFound($"Pago con ID {id} no encontrado");
                }

                return Ok(payment.ToPaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting payment by ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un pago por número de recibo
        /// </summary>
        [HttpGet("receive-number/{receiveNumber}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<PaymentResponse>> GetByReceiveNumber(string receiveNumber)
        {
            try
            {
                Log.Information("GET > Payments > ByReceiveNumber. User: {0}, ReceiveNumber: {1}", User.Identity?.Name, receiveNumber);
                
                var payment = await _paymentRepository.GetByReceiveNumberAsync(receiveNumber);
                if (payment == null)
                {
                    return NotFound($"Pago con número de recibo {receiveNumber} no encontrado");
                }

                return Ok(payment.ToPaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting payment by receive number: {0}", receiveNumber);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene pagos por rango de fechas
        /// </summary>
        [HttpGet("date-range")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<PaymentResponse>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                Log.Information("GET > Payments > ByDateRange. User: {0}, StartDate: {1}, EndDate: {2}", 
                    User.Identity?.Name, startDate, endDate);
                
                if (startDate > endDate)
                {
                    return BadRequest("La fecha de inicio no puede ser mayor que la fecha de fin");
                }

                var payments = await _paymentRepository.GetByDateRangeAsync(startDate, endDate);
                var paymentResponses = payments.Select(p => p.ToPaymentResponse()).ToList();
                
                return Ok(paymentResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting payments by date range: {0} to {1}", startDate, endDate);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo pago (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<PaymentResponse>> Create([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > Payments > Create. User: {0}, Payment: {@Payment}", User.Identity?.Name, paymentRequest);

                // Validar que no existe otro pago con el mismo número de recibo
                var existingPayment = await _paymentRepository.GetByReceiveNumberAsync(paymentRequest.ReceiveNumber);
                if (existingPayment != null)
                {
                    return BadRequest($"Ya existe un pago con el número de recibo {paymentRequest.ReceiveNumber}");
                }

                var payment = paymentRequest.ToPayment();
                await _paymentRepository.AddAsync(payment);
                
                var createdPayment = await _paymentRepository.GetByIdWithRelationsAsync(payment.Id);
                return CreatedAtAction(nameof(GetById), new { id = payment.Id }, createdPayment?.ToPaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating payment");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un pago existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<PaymentResponse>> Update(int id, [FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("PUT > Payments > Update. User: {0}, Id: {1}, Payment: {@Payment}", 
                    User.Identity?.Name, id, paymentRequest);

                var existingPayment = await _paymentRepository.GetByIdAsync(id);
                if (existingPayment == null)
                {
                    return NotFound($"Pago con ID {id} no encontrado");
                }

                // Validar que no existe otro pago con el mismo número de recibo (excepto el actual)
                var duplicatePayment = await _paymentRepository.GetByReceiveNumberAsync(paymentRequest.ReceiveNumber);
                if (duplicatePayment != null && duplicatePayment.Id != id)
                {
                    return BadRequest($"Ya existe otro pago con el número de recibo {paymentRequest.ReceiveNumber}");
                }

                // Actualizar las propiedades
                existingPayment.ReceiveNumber = paymentRequest.ReceiveNumber;
                existingPayment.PaymentDate = paymentRequest.PaymentDate;
                existingPayment.Amount = paymentRequest.Amount;
                existingPayment.Description = paymentRequest.Description;
                existingPayment.ReceivePhoto = paymentRequest.ReceivePhoto;

                await _paymentRepository.UpdateAsync(existingPayment);

                var updatedPayment = await _paymentRepository.GetByIdWithRelationsAsync(id);
                return Ok(updatedPayment?.ToPaymentResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating payment with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un pago (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Log.Information("DELETE > Payments > Delete. User: {0}, Id: {1}", User.Identity?.Name, id);

                var existingPayment = await _paymentRepository.GetByIdAsync(id);
                if (existingPayment == null)
                {
                    return NotFound($"Pago con ID {id} no encontrado");
                }

                // Verificar que no tenga relaciones con gastos
                var paymentWithRelations = await _paymentRepository.GetByIdWithRelationsAsync(id);
                if (paymentWithRelations?.ExpensePayments?.Any() == true)
                {
                    return BadRequest("No se puede eliminar el pago porque tiene relaciones con gastos. " +
                                      "Elimine primero las relaciones gasto-pago.");
                }

                await _paymentRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting payment with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}