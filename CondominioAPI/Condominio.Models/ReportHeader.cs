namespace Condominio.Models;

public partial class ReportHeader
{
  public int Id { get; set; }

  public int ReportId { get; set; }

  public int DisplayOrder { get; set; }

  public int StyleId { get; set; } = -1;

  public string DisplayContent { get; set; } = null!;

  public bool IsQuery { get; set; } = false;

  public DateTime StartDate { get; set; }

  public DateTime? EndDate { get; set; }

  // Navigation properties
  public virtual Report Report { get; set; } = null!;
  public virtual Style? Style { get; set; }
}
