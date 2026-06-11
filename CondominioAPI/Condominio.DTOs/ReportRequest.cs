using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ReportRequest
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
    public string Name { get; set; } = null!;

    public string? HeaderQuery { get; set; }

    [Required(ErrorMessage = "BodyQuery is required")]
    public string BodyQuery { get; set; } = null!;

    public string? FooterQuery { get; set; }
  }
}
