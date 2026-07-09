namespace Condominio.DTOs
{
  public class ReportPdfResponse
  {
    public string FilePath { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public bool Success { get; set; }
    public string? Error { get; set; }

    /// <summary>
    /// Información de almacenamiento en la nube del PDF (si está configurado)
    /// </summary>
    public CloudStorageResponseInfo? CloudStorage { get; set; }
  }

  /// <summary>
  /// Información de almacenamiento en la nube para respuesta HTTP
  /// </summary>
  public class CloudStorageResponseInfo
  {
    public bool IsStored { get; set; }
    public string ProviderName { get; set; }
    public string FileId { get; set; }
    public string FileUrl { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? ErrorMessage { get; set; }
  }
}

