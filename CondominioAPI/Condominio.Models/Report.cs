namespace Condominio.Models;

public partial class Report
{
  public int Id { get; set; }

  public string Name { get; set; } = null!;

  public string? HeaderQuery { get; set; }

  public string BodyQuery { get; set; } = null!;

  public string? FooterQuery { get; set; }

  public virtual ICollection<ReportRole> ReportRoles { get; set; } = new List<ReportRole>();
}
