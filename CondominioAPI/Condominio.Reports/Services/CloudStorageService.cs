using Condominio.Reports.Interfaces;
using Condominio.Reports.Models;
using Condominio.Reports.Providers;
using Serilog;

namespace Condominio.Reports.Services
{
  /// <summary>
  /// Servicio para gestionar operaciones de almacenamiento en la nube
  /// </summary>
  public class CloudStorageService
  {
    private readonly CloudStorageConfig _config;
    private readonly Dictionary<string, ICloudStorageProvider> _providers;

    public CloudStorageService(CloudStorageConfig config)
    {
      _config = config ?? throw new ArgumentNullException(nameof(config));
      _providers = new Dictionary<string, ICloudStorageProvider>(StringComparer.OrdinalIgnoreCase);

      InitializeProviders();
    }

    /// <summary>
    /// Inicializa los proveedores configurados
    /// </summary>
    private void InitializeProviders()
    {
      try
      {
        // Registrar Google Drive si está configurado
        if (_config.GoogleDrive != null && !string.IsNullOrEmpty(_config.GoogleDrive.ClientSecretsPath))
        {
          var googleDriveProvider = new GoogleDriveProvider(_config.GoogleDrive);
          _providers[googleDriveProvider.ProviderName] = googleDriveProvider;
          Log.Information("Google Drive provider registrado");
        }

        // Aquí se pueden agregar más proveedores en el futuro
        // if (_config.OneDrive != null && !string.IsNullOrEmpty(_config.OneDrive.ClientId))
        // {
        //   var oneDriveProvider = new OneDriveProvider(_config.OneDrive);
        //   _providers[oneDriveProvider.ProviderName] = oneDriveProvider;
        // }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error al inicializar proveedores de almacenamiento en la nube");
      }
    }

    /// <summary>
    /// Sube un archivo al proveedor configurado
    /// </summary>
    /// <param name="localFilePath">Ruta local del archivo</param>
    /// <param name="remoteFileName">Nombre del archivo en la nube</param>
    /// <returns>Resultado de la carga</returns>
    public async Task<CloudStorageResult> UploadFileAsync(string localFilePath, string remoteFileName)
    {
      if (!_config.Enabled)
      {
        return new CloudStorageResult
        {
          Success = false,
          ErrorMessage = "Almacenamiento en la nube está deshabilitado"
        };
      }

      try
      {
        var provider = GetActiveProvider();
        if (provider == null)
        {
          return new CloudStorageResult
          {
            Success = false,
            ErrorMessage = $"Proveedor de almacenamiento '{_config.Provider}' no está disponible o no está configurado"
          };
        }

        Log.Information("Iniciando carga de archivo a {0}: {1}", provider.ProviderName, remoteFileName);
        var result = await provider.UploadFileAsync(localFilePath, remoteFileName);

        if (result.Success)
        {
          Log.Information("Archivo cargado exitosamente: {0}", remoteFileName);
        }
        else
        {
          Log.Warning("Error al cargar archivo: {0}", result.ErrorMessage);
        }

        return result;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Excepción durante la carga de archivo a la nube");
        return new CloudStorageResult
        {
          Success = false,
          ErrorMessage = $"Error inesperado: {ex.Message}"
        };
      }
    }

    /// <summary>
    /// Sube un archivo y retorna la información completa
    /// </summary>
    public async Task<UploadCloudFileResult> UploadFileWithInfoAsync(string localFilePath, string remoteFileName)
    {
      var uploadResult = await UploadFileAsync(localFilePath, remoteFileName);

      var result = new UploadCloudFileResult
      {
        Success = uploadResult.Success,
        ErrorMessage = uploadResult.ErrorMessage,
        ProviderName = _config.Provider,
        UploadedAt = uploadResult.UploadedAt
      };

      if (uploadResult.Success)
      {
        result.FileId = uploadResult.FileId;
        result.FileUrl = uploadResult.FileUrl;
        result.LocalFilePath = localFilePath;
      }

      return result;
    }

    /// <summary>
    /// Elimina un archivo del almacenamiento en la nube
    /// </summary>
    public async Task<bool> DeleteFileAsync(string fileId)
    {
      try
      {
        var provider = GetActiveProvider();
        if (provider == null)
          return false;

        return await provider.DeleteFileAsync(fileId);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error al eliminar archivo de la nube");
        return false;
      }
    }

    /// <summary>
    /// Obtiene información del archivo en la nube
    /// </summary>
    public async Task<CloudStorageFileInfo> GetFileInfoAsync(string fileId)
    {
      try
      {
        var provider = GetActiveProvider();
        if (provider == null)
          return null;

        return await provider.GetFileInfoAsync(fileId);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error al obtener información del archivo");
        return null;
      }
    }

    /// <summary>
    /// Verifica si el servicio de almacenamiento en la nube está disponible
    /// </summary>
    public async Task<bool> IsAvailableAsync()
    {
      if (!_config.Enabled)
        return false;

      var provider = GetActiveProvider();
      if (provider == null)
        return false;

      return await provider.IsConfiguredAsync();
    }

    /// <summary>
    /// Obtiene el proveedor activo configurado
    /// </summary>
    private ICloudStorageProvider GetActiveProvider()
    {
      if (string.IsNullOrEmpty(_config.Provider))
      {
        Log.Warning("No hay proveedor configurado");
        return null;
      }

      if (!_providers.TryGetValue(_config.Provider, out var provider))
      {
        Log.Warning("Proveedor no encontrado: {0}", _config.Provider);
        return null;
      }

      return provider;
    }

    /// <summary>
    /// Registra un proveedor personalizado
    /// </summary>
    public void RegisterProvider(ICloudStorageProvider provider)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof(provider));

      _providers[provider.ProviderName] = provider;
      Log.Information("Proveedor registrado: {0}", provider.ProviderName);
    }

    /// <summary>
    /// Obtiene la lista de proveedores disponibles
    /// </summary>
    public IReadOnlyList<string> GetAvailableProviders()
    {
      return _providers.Keys.ToList().AsReadOnly();
    }
  }

  /// <summary>
  /// Resultado detallado de carga de archivo
  /// </summary>
  public class UploadCloudFileResult
  {
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string FileId { get; set; }
    public string FileUrl { get; set; }
    public string ProviderName { get; set; }
    public string LocalFilePath { get; set; }
    public DateTime UploadedAt { get; set; }
  }
}
