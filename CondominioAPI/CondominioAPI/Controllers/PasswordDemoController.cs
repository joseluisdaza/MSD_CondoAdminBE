using Condominio.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordDemoController : ControllerBase
    {
        /// <summary>
        /// Demuestra c�mo BCrypt genera diferentes hashes (con diferentes salts) para la misma contrase�a
        /// </summary>
        [HttpGet("demo-salt")]
        public IActionResult DemoSalt()
        {
            string password = "password123";

            // Hasheamos la misma contrase�a 3 veces
            string hash1 = PasswordHasher.HashPassword(password);
            string hash2 = PasswordHasher.HashPassword(password);
            string hash3 = PasswordHasher.HashPassword(password);

            // Verificamos que todos son v�lidos
            bool verify1 = PasswordHasher.VerifyPassword(password, hash1);
            bool verify2 = PasswordHasher.VerifyPassword(password, hash2);
            bool verify3 = PasswordHasher.VerifyPassword(password, hash3);

            return Ok(new
            {
                message = "La misma contrase�a genera hashes diferentes gracias al SALT �nico",
                originalPassword = password,
                hashes = new
                {
                    hash1 = new
                    {
                        value = hash1,
                        length = hash1.Length,
                        isValid = verify1,
                        salt = ExtractSalt(hash1),
                        explanation = "Cada hash contiene un SALT �nico generado autom�ticamente"
                    },
                    hash2 = new
                    {
                        value = hash2,
                        length = hash2.Length,
                        isValid = verify2,
                        salt = ExtractSalt(hash2),
                        explanation = "Aunque la contrase�a es la misma, el SALT es diferente"
                    },
                    hash3 = new
                    {
                        value = hash3,
                        length = hash3.Length,
                        isValid = verify3,
                        salt = ExtractSalt(hash3),
                        explanation = "Por eso los hashes son diferentes"
                    }
                },
                conclusion = new
                {
                    areHashesDifferent = (hash1 != hash2) && (hash2 != hash3) && (hash1 != hash3),
                    areAllValid = verify1 && verify2 && verify3,
                    security = "Esto protege contra ataques de rainbow table y diccionario"
                }
            });
        }

        /// <summary>
        /// Extrae el SALT de un hash BCrypt (solo con fines demostrativos)
        /// </summary>
        private string ExtractSalt(string hash)
        {
            // El formato de BCrypt es: $2a$11$[22 caracteres de salt][31 caracteres de hash]
            if (hash.Length >= 29)
            {
                // Extrae los caracteres del salt (despu�s de $2a$11$)
                return hash.Substring(7, 22);
            }
            return "Unable to extract";
        }

        /// <summary>
        /// Demuestra la estructura de un hash BCrypt
        /// </summary>
        [HttpGet("demo-structure")]
        public IActionResult DemoStructure()
        {
            string password = "MiContrase�aSegura123!";
            string hash = PasswordHasher.HashPassword(password);

            return Ok(new
            {
                password = password,
                fullHash = hash,
                structure = new
                {
                    algorithm = hash.Substring(0, 4),  // $2a$ o $2b$
                    algorithmExplanation = "Versi�n del algoritmo BCrypt",
                    
                    cost = hash.Substring(4, 3),       // 11$
                    costExplanation = "Factor de trabajo: 2^11 = 2048 iteraciones",
                    
                    salt = hash.Substring(7, 22),      // 22 caracteres
                    saltExplanation = "SALT �nico generado aleatoriamente por BCrypt",
                    
                    actualHash = hash.Substring(29),   // Resto de caracteres
                    actualHashExplanation = "Hash resultante de: BCrypt(password + salt, cost)"
                },
                securityFeatures = new[]
                {
                    "? Salt �nico por contrase�a",
                    "? Generaci�n autom�tica del salt",
                    "? Salt almacenado junto con el hash",
                    "? No requiere gesti�n manual del salt",
                    "? Protecci�n contra rainbow tables",
                    "? Protecci�n contra ataques de diccionario"
                }
            });
        }
    }
}
