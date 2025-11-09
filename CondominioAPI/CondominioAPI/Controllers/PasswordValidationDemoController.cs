using Condominio.DTOs.Validation;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordValidationDemoController : ControllerBase
    {
        /// <summary>
        /// Endpoint para probar la validación de contraseñas antes de crear un usuario
        /// </summary>
        /// <param name="request">Contraseña a validar</param>
        /// <returns>Resultado detallado de la validación</returns>
        [HttpPost("validate")]
        public IActionResult ValidatePassword([FromBody] PasswordTestRequest request)
        {
            Log.Information("GET > PasswordValidation > ValidatePassword");

            var result = StrongPasswordAttribute.ValidatePasswordStrength(request.Password);

            return Ok(new
            {
                password = request.Password,
                isValid = result.IsValid,
                summary = result.IsValid 
                    ? "? La contraseña cumple con todos los requisitos de seguridad" 
                    : "? La contraseña no cumple con los requisitos de seguridad",
                requirements = result.Requirements.Select(r => new
                {
                    requirement = r.Name,
                    isMet = r.IsMet,
                    status = r.IsMet ? "?" : "?",
                    description = r.Description
                })
            });
        }

        /// <summary>
        /// Obtiene los requisitos de contraseña sin validar ninguna contraseña específica
        /// </summary>
        [HttpGet("requirements")]
        public IActionResult GetPasswordRequirements()
        {
            Log.Information("GET > PasswordValidation > Requirements");
            return Ok(new
            {
                title = "Requisitos de Contraseña Segura",
                requirements = new[]
                {
                    new { name = "Longitud mínima", value = "12 caracteres", icon = "??" },
                    new { name = "Números", value = "Al menos 1 número (0-9)", icon = "1??" },
                    new { name = "Mayúsculas", value = "Al menos 1 letra mayúscula (A-Z)", icon = "??" },
                    new { name = "Caracteres especiales", value = "Al menos 1 caracter especial (!@#$%^&*...)", icon = "?" }
                },
                examples = new
                {
                    valid = new[]
                    {
                        "MySecurePass123!",
                        "C0mpl3x&Strong",
                        "Password2025#Secure"
                    },
                    invalid = new[]
                    {
                        new { password = "short", reason = "Menos de 12 caracteres" },
                        new { password = "NoNumbersHere!", reason = "No contiene números" },
                        new { password = "no-uppercase-123", reason = "No contiene mayúsculas" },
                        new { password = "NoSpecialChar123", reason = "No contiene caracteres especiales" }
                    }
                }
            });
        }
    }

    public class PasswordTestRequest
    {
        public string Password { get; set; } = string.Empty;
    }
}
