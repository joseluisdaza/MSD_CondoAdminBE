using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class AuditLogRequest
{
    [Required(ErrorMessage = "El ID del usuario es requerido")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "La acción es requerida")]
    [StringLength(100, ErrorMessage = "La acción no puede exceder 100 caracteres")]
    public string Action { get; set; } = null!;

    [StringLength(100, ErrorMessage = "El nombre de tabla no puede exceder 100 caracteres")]
    public string? TableName { get; set; }

    [Required(ErrorMessage = "El mensaje es requerido")]
    public string Message { get; set; } = null!;

    public DateTime? Timestamp { get; set; }
}

public class AuditLogResponse : AuditLogRequest
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? UserLogin { get; set; }

    public new DateTime Timestamp { get; set; }
}

public class AuditLogFilterRequest
{
    public int? UserId { get; set; }
    public string? Action { get; set; }
    public string? TableName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? PageSize { get; set; } = 50;
    public int? Page { get; set; } = 1;
}