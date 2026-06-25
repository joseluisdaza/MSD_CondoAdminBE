using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ReportFooterLightRequest
  {
        [Required(ErrorMessage = "DisplayOrder is required")]
    public int DisplayOrder { get; set; }

    public int StyleId { get; set; } = -1;

    [Required(ErrorMessage = "DisplayContent is required")]
    public string DisplayContent { get; set; } = null!;

    public bool IsQuery { get; set; } = false;

  }

  public class ReportFooterRequest
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "ReportId is required")]
    public int ReportId { get; set; }

    [Required(ErrorMessage = "DisplayOrder is required")]
    public int DisplayOrder { get; set; }

    public int StyleId { get; set; } = -1;

    [Required(ErrorMessage = "DisplayContent is required")]
    public string DisplayContent { get; set; } = null!;

    public bool IsQuery { get; set; } = false;

    public DateTime StartDate { get; set; } = DateTime.Now;

    public DateTime? EndDate { get; set; }
  }

  public class ReportFooterResponse
  {
    public int Id { get; set; }

    public int ReportId { get; set; }

    public int DisplayOrder { get; set; }

    public int StyleId { get; set; }

    public string DisplayContent { get; set; } = null!;

    public bool IsQuery { get; set; }

    public StyleResponse? Style { get; set; }
  }
}
