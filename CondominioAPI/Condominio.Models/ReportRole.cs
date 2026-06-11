namespace Condominio.Models;

public partial class ReportRole
{
  public int ReportId { get; set; }

  public int RoleId { get; set; }

  public virtual Report Report { get; set; } = null!;

  public virtual Role Role { get; set; } = null!;
}
