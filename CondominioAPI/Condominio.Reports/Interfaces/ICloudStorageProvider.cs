namespace Condominio.Reports.Interfaces
{
  /// <summary>
  /// Interface que define un proveedor de almacenamiento en la nube.
  /// Las implementaciones pueden ser para Google Drive, OneDrive, AWS S3, etc.
  /// </summary>
  public interface ICloudStorageProvider
  {
    /// <summary>
    /// Nombre identificador del proveedor
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Sube un archivo a la nube
    /// </summary>
    /// <param name="localFilePath">Ruta local del archivo a subir</param>
    /// <param name="remoteFileName">Nombre del archivo en la nube</param>
    /// <returns>URL o identificador del archivo en la nube</returns>
    Task<CloudStorageResult> UploadFileAsync(string localFilePath, string remoteFileName);

    /// <summary>
    /// Elimina un archivo de la nube
    /// </summary>
    /// <param name="fileId">Identificador del archivo en la nube</param>
    /// <returns>True si se eliminó exitosamente, false en caso contrario</returns>
    Task<bool> DeleteFileAsync(string fileId);

    /// <summary>
    /// Obtiene información del archivo
    /// </summary>
    /// <param name="fileId">Identificador del archivo</param>
    /// <returns>Información del archivo</returns>
    Task<CloudStorageFileInfo> GetFileInfoAsync(string fileId);

    /// <summary>
    /// Verifica si el proveedor está correctamente configurado
    /// </summary>
    /// <returns>True si está configurado, false en caso contrario</returns>
    Task<bool> IsConfiguredAsync();
  }

  /// <summary>
  /// Resultado de una operación de carga en la nube
  /// </summary>
  public class CloudStorageResult
  {
    public bool Success { get; set; }
    public string FileId { get; set; }
    public string FileUrl { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime UploadedAt { get; set; }

    public CloudStorageResult()
    {
      UploadedAt = DateTime.UtcNow;
    }
  }

  /// <summary>
  /// Información de un archivo en la nube
  /// </summary>
  public class CloudStorageFileInfo
  {
    public string FileId { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public long SizeInBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public string MimeType { get; set; }
  }
}
