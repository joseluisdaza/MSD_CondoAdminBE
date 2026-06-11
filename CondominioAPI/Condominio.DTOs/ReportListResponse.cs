using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ReportListResponse
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
  }
}
