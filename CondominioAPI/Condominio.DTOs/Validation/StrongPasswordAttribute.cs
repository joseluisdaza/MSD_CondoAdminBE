using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs.Validation
{
    /// <summary>
    /// Valida que una contraseña cumpla con los requisitos de complejidad alta.
    /// Requisitos:
    /// - Al menos 12 caracteres
    /// - Al menos 1 número
    /// - Al menos 1 letra mayúscula
    /// - Al menos 1 caracter especial (no espacio)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class StrongPasswordAttribute : ValidationAttribute
    {
        private const int MinimumLength = 12;

        public StrongPasswordAttribute()
        {
            ErrorMessage = "La contraseña no cumple con los requisitos de seguridad";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("La contraseña es requerida");
            }

            var errors = new List<string>();

            // Validar longitud mínima
            if (password.Length < MinimumLength)
            {
                errors.Add($"al menos {MinimumLength} caracteres");
            }

            // Validar que contenga al menos un número
            if (!password.Any(char.IsDigit))
            {
                errors.Add("al menos 1 número");
            }

            // Validar que contenga al menos una letra mayúscula
            if (!password.Any(char.IsUpper))
            {
                errors.Add("al menos 1 letra mayúscula");
            }

            // Validar que contenga al menos un caracter especial (no letra, no dígito, no espacio)
            if (!password.Any(ch => !char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch)))
            {
                errors.Add("al menos 1 caracter especial (!@#$%^&*...)");
            }

            if (errors.Any())
            {
                return new ValidationResult(
                    $"La contraseña debe contener {string.Join(", ", errors)}",
                    new[] { validationContext.MemberName ?? "Password" }
                );
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Valida una contraseña y retorna información detallada sobre su fortaleza
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
                    Name = "Contraseña requerida",
                    IsMet = false,
                    Description = "La contraseña no puede estar vacía"
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

            // Número
            var hasDigit = password.Any(char.IsDigit);
            result.Requirements.Add(new PasswordRequirement
            {
                Name = "Número",
                IsMet = hasDigit,
                Description = "Al menos 1 número (0-9)"
            });
            if (!hasDigit) result.IsValid = false;

            // Mayúscula
            var hasUpper = password.Any(char.IsUpper);
            result.Requirements.Add(new PasswordRequirement
            {
                Name = "Mayúscula",
                IsMet = hasUpper,
                Description = "Al menos 1 letra mayúscula (A-Z)"
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
    /// Resultado de la validación de contraseña con detalles
    /// </summary>
    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public List<PasswordRequirement> Requirements { get; set; } = new();
    }

    /// <summary>
    /// Requisito individual de contraseña
    /// </summary>
    public class PasswordRequirement
    {
        public string Name { get; set; } = string.Empty;
        public bool IsMet { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
