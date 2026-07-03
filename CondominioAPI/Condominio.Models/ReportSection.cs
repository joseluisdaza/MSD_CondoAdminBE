namespace Condominio.Models;

public partial class ReportSection : AbstractReportPart
{
  public int HeaderStyleId { get; set; } = -1;

  //// Navigation properties
  public virtual Style? HeaderStyle { get; set; }
}
