using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class PropertyOwnerRequest
{
    [Required(ErrorMessage = "El ID de la propiedad es requerido")]
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public int UserId { get; set; }
}

public class PropertyOwnerResponse
{
    public int PropertyId { get; set; }
    public int UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Información adicional de la propiedad
    public string? PropertyLegalId { get; set; }
    public string? PropertyTower { get; set; }
    public int? PropertyFloor { get; set; }
    public string? PropertyCode { get; set; }

    // Información adicional del usuario
    public string? UserName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserLogin { get; set; }
    public string? UserLegalId { get; set; }

    public bool IsActive => EndDate == null;
}

public class PropertyOwnerFilterRequest
{
    public int? PropertyId { get; set; }
    public int? UserId { get; set; }
    public bool IncludeFinalized { get; set; } = false;
}