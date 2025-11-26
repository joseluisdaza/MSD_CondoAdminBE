namespace Condominio.Models;

public partial class AuditLog
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = null!;

    public string? TableName { get; set; }

    public string Message { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}