using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ReportDataRequest
  {
    [Required(ErrorMessage = "ReportId is required")]
    public int ReportId { get; set; }

    public Dictionary<string, object>? Filters { get; set; }
  }
}
