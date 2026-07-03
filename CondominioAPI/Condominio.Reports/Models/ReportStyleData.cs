namespace Condominio.Reports
{
  /// <summary>
  /// Modelo que representa un estilo disponible para usar en el reporte.
  /// Contiene toda la información de formato que puede aplicarse a diferentes partes del reporte.
  /// </summary>
  public class ReportStyleData
  {
    /// <summary>
    /// ID único del estilo
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del estilo
    /// </summary>
    public string StyleName { get; set; } = null!;

    /// <summary>
    /// Indica si el texto debe estar en negrita
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Indica si el texto debe estar en cursiva
    /// </summary>
    public bool Italic { get; set; }

    /// <summary>
    /// Indica si el texto debe estar subrayado
    /// </summary>
    public bool Underline { get; set; }

    /// <summary>
    /// Tamaño de la fuente en puntos
    /// </summary>
    public int FontSize { get; set; }

    /// <summary>
    /// Color de la fuente (formato hexadecimal, ej: #000000)
    /// </summary>
    public string? FontColor { get; set; }

    /// <summary>
    /// Color de fondo (formato hexadecimal, ej: #FFFFFF)
    /// </summary>
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Alineación horizontal (left, center, right)
    /// </summary>
    public string? HorizontalAlignment { get; set; }

    /// <summary>
    /// Alineación vertical (top, middle, bottom)
    /// </summary>
    public string? VerticalAlignment { get; set; }

    /// <summary>
    /// Porcentaje de ancho (para tablas)
    /// </summary>
    public decimal WidthPercentage { get; set; }
  }
}
