namespace Condominio.Models;

public partial class Style
{
  public int Id { get; set; }

  public string StyleName { get; set; } = null!;

  public bool Bold { get; set; } = false;

  public bool Italic { get; set; } = false;

  public bool Underline { get; set; } = false;

  public int FontSize { get; set; } = 12;

  public string FontColor { get; set; } = "#000000";

  public string BackgroundColor { get; set; } = "#FFFFFF";

  public string HorizontalAlignment { get; set; } = "left";

  public string VerticalAlignment { get; set; } = "top";

  public DateTime StartDate { get; set; }

  public DateTime? EndDate { get; set; }

  public int WidthPercentage { get; set; } = 100;

  // Navigation properties
  public virtual ICollection<ReportHeader> ReportHeaders { get; set; } = new List<ReportHeader>();
  public virtual ICollection<ReportSection> ReportSections { get; set; } = new List<ReportSection>();
  public virtual ICollection<ReportSection> ReportSectionsHeaderStyle { get; set; } = new List<ReportSection>();
  public virtual ICollection<ReportFooter> ReportFooters { get; set; } = new List<ReportFooter>();
}
