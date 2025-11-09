using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthHealthCheckController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            Log.Information("GET > AuthHealthCheck");
            var username = User.Identity?.Name ?? "desconocido";
            return Ok(new { mensaje = $"Hello, {username}!" });
        }
    }
}
