namespace Condominio.Models;

public partial class ReportParam
{
  public int Id { get; set; }

  public int ReportId { get; set; }

  public string ParamName { get; set; } = null!;

  public string ParamType { get; set; } = null!;

  public string? ParamDescription { get; set; }

  public DateTime StartDate { get; set; }

  public DateTime? EndDate { get; set; }

  // Navigation properties
  public virtual Report Report { get; set; } = null!;
}
