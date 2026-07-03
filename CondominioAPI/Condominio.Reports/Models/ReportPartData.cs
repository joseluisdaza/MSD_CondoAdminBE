namespace Condominio.Reports
{
  /// <summary>
  /// Modelo que representa una parte del reporte (header, section, footer).
  /// Contiene el contenido y metadatos de renderizado.
  /// </summary>
  public class ReportPartData
  {
    /// <summary>
    /// Contenido a mostrar (puede ser texto estático o resultado de query)
    /// </summary>
    public object? Content { get; set; }

    /// <summary>
    /// ID del estilo a aplicar a esta parte
    /// </summary>
    public int StyleId { get; set; }

    /// <summary>
    /// Indica si el contenido es una tabla (resultado de query) o texto estático
    /// </summary>
    public bool IsTable { get; set; }

    /// <summary>
    /// Orden de visualización de esta parte en el reporte
    /// </summary>
    public int DisplayOrder { get; set; }
  }
}
