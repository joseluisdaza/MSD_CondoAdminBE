using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ReportListResponse
  {
    public int Id { get; set; }

    public string ReportName { get; set; } = null!;

    public string DisplayName { get; set; } = null!;
    
    public List<ReportParamLightResponse> Params { get; set; } = new List<ReportParamLightResponse>();
  }
}
