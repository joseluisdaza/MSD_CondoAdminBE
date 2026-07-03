namespace Condominio.Reports
{
  /// <summary>
  /// Modelo de datos que contiene la información agnóstica de base de datos necesaria para generar un reporte.
  /// Esta clase es la que se pasa al generador, permitiendo que éste sea independiente de la capa de datos.
  /// </summary>
  public class ReportExecutionData
  {
    /// <summary>
    /// Título del reporte
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// ID del estilo del título
    /// </summary>
    public int TitleStyleId { get; set; }

    /// <summary>
    /// Lista de estilos disponibles para usar en el reporte
    /// </summary>
    public IEnumerable<ReportStyleData> AvailableStyles { get; set; } = new List<ReportStyleData>();

    /// <summary>
    /// Partes del header del reporte
    /// </summary>
    public IEnumerable<ReportPartData> HeaderParts { get; set; } = new List<ReportPartData>();

    /// <summary>
    /// Partes de las secciones del reporte
    /// </summary>
    public IEnumerable<ReportPartData> SectionParts { get; set; } = new List<ReportPartData>();

    /// <summary>
    /// Partes del footer del reporte
    /// </summary>
    public IEnumerable<ReportPartData> FooterParts { get; set; } = new List<ReportPartData>();
  }
}
