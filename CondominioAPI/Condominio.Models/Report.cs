namespace Condominio.Models;

public partial class Report
{
  public int Id { get; set; }

  public string ReportName { get; set; } = null!;

  public string DisplayName { get; set; } = null!;

  public int TitleStyleId { get; set; } = -1;

  public bool DisplayHeader { get; set; } = true;

  public bool DisplayFooter { get; set; } = true;

  public DateTime StartDate { get; set; }

  public DateTime? EndDate { get; set; }

  // Navigation properties
  public virtual Style? TitleStyle { get; set; }
  public virtual ICollection<ReportRole> ReportRoles { get; set; } = new List<ReportRole>();
  public virtual ICollection<ReportHeader> ReportHeaders { get; set; } = new List<ReportHeader>();
  public virtual ICollection<ReportSection> ReportSections { get; set; } = new List<ReportSection>();
  public virtual ICollection<ReportFooter> ReportFooters { get; set; } = new List<ReportFooter>();
  public virtual ICollection<ReportAudit> ReportAudits { get; set; } = new List<ReportAudit>();
  public virtual ICollection<ReportParam> ReportParams { get; set; } = new List<ReportParam>();
}
