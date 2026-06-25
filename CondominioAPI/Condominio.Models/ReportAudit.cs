namespace Condominio.Models;

public partial class ReportAudit
{
  public int Id { get; set; }

  public int ReportId { get; set; }

  public int UserId { get; set; }

  public string Parameters { get; set; } = null!;

  public DateTime ExecutionDate { get; set; }

  // Navigation properties
  public virtual Report Report { get; set; } = null!;
  public virtual User User { get; set; } = null!;
}
