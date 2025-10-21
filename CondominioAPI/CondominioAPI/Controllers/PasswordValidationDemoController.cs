using Condominio.DTOs.Validation;
using Microsoft.AspNetCore.Mvc;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordValidationDemoController : ControllerBase
    {
        /// <summary>
        /// Endpoint para probar la validaci�n de contrase�as antes de crear un usuario
        /// </summary>
        /// <param name="request">Contrase�a a validar</param>
        /// <returns>Resultado detallado de la validaci�n</returns>
        [HttpPost("validate")]
        public IActionResult ValidatePassword([FromBody] PasswordTestRequest request)
        {
            var result = StrongPasswordAttribute.ValidatePasswordStrength(request.Password);

            return Ok(new
            {
                password = request.Password,
                isValid = result.IsValid,
                summary = result.IsValid 
                    ? "? La contrase�a cumple con todos los requisitos de seguridad" 
                    : "? La contrase�a no cumple con los requisitos de seguridad",
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
        /// Obtiene los requisitos de contrase�a sin validar ninguna contrase�a espec�fica
        /// </summary>
        [HttpGet("requirements")]
        public IActionResult GetPasswordRequirements()
        {
            return Ok(new
            {
                title = "Requisitos de Contrase�a Segura",
                requirements = new[]
                {
                    new { name = "Longitud m�nima", value = "12 caracteres", icon = "??" },
                    new { name = "N�meros", value = "Al menos 1 n�mero (0-9)", icon = "1??" },
                    new { name = "May�sculas", value = "Al menos 1 letra may�scula (A-Z)", icon = "??" },
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
                        new { password = "NoNumbersHere!", reason = "No contiene n�meros" },
                        new { password = "no-uppercase-123", reason = "No contiene may�sculas" },
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
