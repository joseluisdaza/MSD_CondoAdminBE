using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{

  public class ReportLightRequest
  {
    [Required(ErrorMessage = "ReportName is required")]
    [MaxLength(50, ErrorMessage = "ReportName cannot exceed 50 characters")]
    public string ReportName { get; set; } = null!;

    [Required(ErrorMessage = "DisplayName is required")]
    [MaxLength(150, ErrorMessage = "DisplayName cannot exceed 150 characters")]
    public string DisplayName { get; set; } = null!;

    public int TitleStyleId { get; set; } = -1;

    public bool DisplayHeader { get; set; } = true;

    public bool DisplayFooter { get; set; } = true;

    public IEnumerable<ReportHeaderLightRequest> Headers { get; set; } = new List<ReportHeaderLightRequest>();
    public IEnumerable<ReportSectionLightRequest> Sections { get; set; } = new List<ReportSectionLightRequest>();
    public IEnumerable<ReportFooterLightRequest> Footers { get; set; } = new List<ReportFooterLightRequest>();
    public IEnumerable<ReportParamLightRequest> Params { get; set; } = new List<ReportParamLightRequest>();
  }

  public class ReportRequest
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "ReportName is required")]
    [MaxLength(50, ErrorMessage = "ReportName cannot exceed 50 characters")]
    public string ReportName { get; set; } = null!;

    [Required(ErrorMessage = "DisplayName is required")]
    [MaxLength(150, ErrorMessage = "DisplayName cannot exceed 150 characters")]
    public string DisplayName { get; set; } = null!;

    public int TitleStyleId { get; set; } = -1;

    public bool DisplayHeader { get; set; } = true;

    public bool DisplayFooter { get; set; } = true;

    public DateTime StartDate { get; set; } = DateTime.Now;

    public DateTime? EndDate { get; set; }

    public IEnumerable<ReportParamRequest> Params { get; set; } = new List<ReportParamRequest>();
  }

  public class ReportDetailResponse
  {
    public int Id { get; set; }

    public string ReportName { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public int TitleStyleId { get; set; }

    public bool DisplayHeader { get; set; }

    public bool DisplayFooter { get; set; }

    public StyleResponse? TitleStyle { get; set; }

    public IEnumerable<ReportHeaderResponse> Headers { get; set; } = new List<ReportHeaderResponse>();

    public IEnumerable<ReportSectionResponse> Sections { get; set; } = new List<ReportSectionResponse>();

    public IEnumerable<ReportFooterResponse> Footers { get; set; } = new List<ReportFooterResponse>();

    public IEnumerable<ReportParamResponse> Params { get; set; } = new List<ReportParamResponse>();
  }

  public class ReportLightResponse
  {
    public int Id { get; set; }
    public string ReportName { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public int TitleStyleId { get; set; }
    public bool DisplayHeader { get; set; }
    public bool DisplayFooter { get; set; }
    public StyleResponse? TitleStyle { get; set; }
    public IEnumerable<ReportParamResponse> Params { get; set; } = new List<ReportParamResponse>();
  }

  public class ReportParamLightRequest
  {
    [Required(ErrorMessage = "ParamName is required")]
    [MaxLength(100, ErrorMessage = "ParamName cannot exceed 100 characters")]
    public string ParamName { get; set; } = null!;

    [Required(ErrorMessage = "ParamType is required")]
    [MaxLength(50, ErrorMessage = "ParamType cannot exceed 50 characters")]
    public string ParamType { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "ParamDescription cannot exceed 500 characters")]
    public string? ParamDescription { get; set; }

  }

  public class ReportParamRequest : ReportParamLightRequest
  {
    public DateTime StartDate { get; set; } = DateTime.Now;

    public DateTime? EndDate { get; set; }
  }

  public class ReportParamLightResponse
  {
    public string ParamName { get; set; } = null!;

    public string ParamType { get; set; } = null!;

    public string? ParamDescription { get; set; }
  }

  public class ReportParamResponse : ReportParamLightResponse
  {
    public int Id { get; set; }

    public int ReportId { get; set; }
  }
  
}
