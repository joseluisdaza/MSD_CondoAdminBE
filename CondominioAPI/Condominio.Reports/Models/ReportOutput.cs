using Condominio.DTOs;

namespace Condominio.Reports.Models
{
  public abstract class AbstractReportOutput
  {
    public bool Success { get; set; }
  }

  public class JsonReportOutput : AbstractReportOutput
  {
    public ReportExecutionResponse Content { get; set; }
  }

  public class FileReportOutput : AbstractReportOutput
  {
    public string FilePath { get; set; }
    public string FileName { get; set; }

    /// <summary>
    /// Información de almacenamiento en la nube (si está configurado)
    /// </summary>
    public CloudStorageInfo CloudStorage { get; set; }
  }

  /// <summary>
  /// Información sobre el almacenamiento en la nube del reporte
  /// </summary>
  public class CloudStorageInfo
  {
    /// <summary>
    /// Indica si se almacenó en la nube
    /// </summary>
    public bool IsStored { get; set; }

    /// <summary>
    /// Nombre del proveedor de almacenamiento
    /// </summary>
    public string ProviderName { get; set; }

    /// <summary>
    /// ID único del archivo en la nube
    /// </summary>
    public string FileId { get; set; }

    /// <summary>
    /// URL de acceso al archivo en la nube
    /// </summary>
    public string FileUrl { get; set; }

    /// <summary>
    /// Fecha y hora de carga a la nube
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Mensaje de error si la carga falló
    /// </summary>
    public string ErrorMessage { get; set; }
  }
}

