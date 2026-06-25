using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class StyleRequest
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "StyleName is required")]
    [MaxLength(100, ErrorMessage = "StyleName cannot exceed 100 characters")]
    public string StyleName { get; set; } = null!;

    public bool Bold { get; set; } = false;

    public bool Italic { get; set; } = false;

    public bool Underline { get; set; } = false;

    public int FontSize { get; set; } = 12;

    public string FontColor { get; set; } = "#000000";

    public string BackgroundColor { get; set; } = "#FFFFFF";

    public string HorizontalAlignment { get; set; } = "left";

    public string VerticalAlignment { get; set; } = "top";

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int WidthPercentage { get; set; } = 100;
  }

  public class StyleResponse
  {
    public int Id { get; set; }

    public string StyleName { get; set; } = null!;

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underline { get; set; }

    public int FontSize { get; set; }

    public string FontColor { get; set; } = null!;

    public string BackgroundColor { get; set; } = null!;

    public string HorizontalAlignment { get; set; } = null!;

    public string VerticalAlignment { get; set; } = null!;

    public int WidthPercentage { get; set; }
  }
}
