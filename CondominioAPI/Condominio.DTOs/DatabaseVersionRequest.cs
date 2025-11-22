using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class DatabaseVersionRequest
{
    [Required(ErrorMessage = "El número de versión es requerido")]
    [StringLength(20, ErrorMessage = "El número de versión no puede exceder 20 caracteres")]
    public string VersionNumber { get; set; } = null!;

    [Required(ErrorMessage = "La fecha de actualización es requerida")]
    public DateTime LastUpdated { get; set; }
}

public class DatabaseVersionResponse : DatabaseVersionRequest
{
}