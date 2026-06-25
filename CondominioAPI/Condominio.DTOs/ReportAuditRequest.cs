using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ReportAuditRequest
  {
    public int ReportId { get; set; }

    public int UserId { get; set; }

    public string Parameters { get; set; } = null!;
  }

  public class ReportAuditResponse
  {
    public int Id { get; set; }

    public int ReportId { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Parameters { get; set; } = null!;

    public DateTime ExecutionDate { get; set; }
  }
}
