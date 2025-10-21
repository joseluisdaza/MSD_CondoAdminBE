using Condominio.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordDemoController : ControllerBase
    {
        /// <summary>
        /// Demuestra cómo BCrypt genera diferentes hashes (con diferentes salts) para la misma contraseña
        /// </summary>
        [HttpGet("demo-salt")]
        public IActionResult DemoSalt()
        {
            string password = "password123";

            // Hasheamos la misma contraseña 3 veces
            string hash1 = PasswordHasher.HashPassword(password);
            string hash2 = PasswordHasher.HashPassword(password);
            string hash3 = PasswordHasher.HashPassword(password);

            // Verificamos que todos son válidos
            bool verify1 = PasswordHasher.VerifyPassword(password, hash1);
            bool verify2 = PasswordHasher.VerifyPassword(password, hash2);
            bool verify3 = PasswordHasher.VerifyPassword(password, hash3);

            return Ok(new
            {
                message = "La misma contraseña genera hashes diferentes gracias al SALT único",
                originalPassword = password,
                hashes = new
                {
                    hash1 = new
                    {
                        value = hash1,
                        length = hash1.Length,
                        isValid = verify1,
                        salt = ExtractSalt(hash1),
                        explanation = "Cada hash contiene un SALT único generado automáticamente"
                    },
                    hash2 = new
                    {
                        value = hash2,
                        length = hash2.Length,
                        isValid = verify2,
                        salt = ExtractSalt(hash2),
                        explanation = "Aunque la contraseña es la misma, el SALT es diferente"
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
                // Extrae los caracteres del salt (después de $2a$11$)
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
            string password = "MiContraseñaSegura123!";
            string hash = PasswordHasher.HashPassword(password);

            return Ok(new
            {
                password = password,
                fullHash = hash,
                structure = new
                {
                    algorithm = hash.Substring(0, 4),  // $2a$ o $2b$
                    algorithmExplanation = "Versión del algoritmo BCrypt",
                    
                    cost = hash.Substring(4, 3),       // 11$
                    costExplanation = "Factor de trabajo: 2^11 = 2048 iteraciones",
                    
                    salt = hash.Substring(7, 22),      // 22 caracteres
                    saltExplanation = "SALT único generado aleatoriamente por BCrypt",
                    
                    actualHash = hash.Substring(29),   // Resto de caracteres
                    actualHashExplanation = "Hash resultante de: BCrypt(password + salt, cost)"
                },
                securityFeatures = new[]
                {
                    "? Salt único por contraseña",
                    "? Generación automática del salt",
                    "? Salt almacenado junto con el hash",
                    "? No requiere gestión manual del salt",
                    "? Protección contra rainbow tables",
                    "? Protección contra ataques de diccionario"
                }
            });
        }
    }
}
