using System.ComponentModel.DataAnnotations;
using Condominio.DTOs.Validation;

namespace Condominio.DTOs;

public class UpdatePasswordRequest
{
    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [StrongPassword]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
    [Compare(nameof(NewPassword), ErrorMessage = "La confirmación de contraseña no coincide")]
    public string ConfirmPassword { get; set; } = null!;
}