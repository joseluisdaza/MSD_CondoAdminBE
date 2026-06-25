using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ReportExecutionRequest
  {
    [Required(ErrorMessage = "Filters are required")]
    public Dictionary<string, object>? Filters { get; set; }
  }
}
