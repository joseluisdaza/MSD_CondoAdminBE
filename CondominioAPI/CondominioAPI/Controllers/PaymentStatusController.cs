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
    public class PaymentStatusController : ControllerBase
    {
        private readonly IPaymentStatusRepository _paymentStatusRepository;

        public PaymentStatusController(IPaymentStatusRepository paymentStatusRepository)
        {
            _paymentStatusRepository = paymentStatusRepository;
        }

        /// <summary>
        /// Obtiene todos los estados de pago disponibles (Id y StatusDescription únicamente)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<PaymentStatusSimpleResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > PaymentStatus > GetAll. User: {0}", User.Identity?.Name);
                
                var paymentStatuses = await _paymentStatusRepository.GetAllAsync();
                var simpleResponses = paymentStatuses.Select(ps => ps.ToPaymentStatusSimpleResponse()).ToList();
                
                Log.Information("PaymentStatus GetAll completed successfully. Count: {0}", simpleResponses.Count);
                
                return Ok(simpleResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all payment statuses");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}