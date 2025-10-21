using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs.Validation
{
    /// <summary>
    /// Valida que una contrase�a cumpla con los requisitos de complejidad alta.
    /// Requisitos:
    /// - Al menos 12 caracteres
    /// - Al menos 1 n�mero
    /// - Al menos 1 letra may�scula
    /// - Al menos 1 caracter especial (no espacio)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class StrongPasswordAttribute : ValidationAttribute
    {
        private const int MinimumLength = 12;

        public StrongPasswordAttribute()
        {
            ErrorMessage = "La contrase�a no cumple con los requisitos de seguridad";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("La contrase�a es requerida");
            }

            var errors = new List<string>();

            // Validar longitud m�nima
            if (password.Length < MinimumLength)
            {
                errors.Add($"al menos {MinimumLength} caracteres");
            }

            // Validar que contenga al menos un n�mero
            if (!password.Any(char.IsDigit))
            {
                errors.Add("al menos 1 n�mero");
            }

            // Validar que contenga al menos una letra may�scula
            if (!password.Any(char.IsUpper))
            {
                errors.Add("al menos 1 letra may�scula");
            }

            // Validar que contenga al menos un caracter especial (no letra, no d�gito, no espacio)
            if (!password.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch)))
            {
                errors.Add("al menos 1 caracter especial (!@#$%^&*...)");
            }

            if (errors.Any())
            {
                return new ValidationResult(
                    $"La contrase�a debe contener {string.Join(", ", errors)}",
                    new[] { validationContext.MemberName ?? "Password" }
                );
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Valida una contrase�a y retorna informaci�n detallada sobre su fortaleza
        /// </summary>
        public static PasswordValidationResult ValidatePasswordStrength(string password)
        {
            var result = new PasswordValidationResult
            {
                IsValid = true,
                Requirements = new List<PasswordRequirement>()
            };

            if (string.IsNullOrEmpty(password))
            {
                result.IsValid = false;
                result.Requirements.Add(new PasswordRequirement
                {
                    Name = "Contrase�a requerida",
                    IsMet = false,
                    Description = "La contrase�a no puede estar vac�a"
                });
                return result;
            }

            // Longitud
            var hasMinLength = password.Length >= MinimumLength;
            result.Requirements.Add(new PasswordRequirement
            {
                Name = "Longitud",
                IsMet = hasMinLength,
                Description = $"Al menos {MinimumLength} caracteres (actual: {password.Length})"
            });
            if (!hasMinLength) result.IsValid = false;

            // N�mero
            var hasDigit = password.Any(char.IsDigit);
            result.Requirements.Add(new PasswordRequirement
            {
                Name = "N�mero",
                IsMet = hasDigit,
                Description = "Al menos 1 n�mero (0-9)"
            });
            if (!hasDigit) result.IsValid = false;

            // May�scula
            var hasUpper = password.Any(char.IsUpper);
            result.Requirements.Add(new PasswordRequirement
            {
                Name = "May�scula",
                IsMet = hasUpper,
                Description = "Al menos 1 letra may�scula (A-Z)"
            });
            if (!hasUpper) result.IsValid = false;

            // Caracter especial
            var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch));
            result.Requirements.Add(new PasswordRequirement
            {
                Name = "Caracter especial",
                IsMet = hasSpecial,
                Description = "Al menos 1 caracter especial (!@#$%^&*...)"
            });
            if (!hasSpecial) result.IsValid = false;

            return result;
        }
    }

    /// <summary>
    /// Resultado de la validaci�n de contrase�a con detalles
    /// </summary>
    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public List<PasswordRequirement> Requirements { get; set; } = new();
    }

    /// <summary>
    /// Requisito individual de contrase�a
    /// </summary>
    public class PasswordRequirement
    {
        public string Name { get; set; } = string.Empty;
        public bool IsMet { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
