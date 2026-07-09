using Condominio.Reports.Interfaces;
using Condominio.Reports.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Serilog;
using System.Net.Http;
using System.Text.Json;

namespace Condominio.Reports.Providers
{
  /// <summary>
  /// Proveedor de almacenamiento en Google Drive usando OAuth2 personal con Refresh Token
  /// </summary>
  public class GoogleDriveProvider : ICloudStorageProvider
  {
    public string ProviderName => "GoogleDrive";

    private readonly GoogleDriveConfig _config;
    private DriveService _driveService;
    private string _rootFolderId;

    public GoogleDriveProvider(GoogleDriveConfig config)
    {
      _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Crea una credencial a partir del refresh token
    /// </summary>
    private async Task<UserCredential> CreateCredentialFromRefreshTokenAsync(string clientSecretsPath, string refreshToken)
    {
      using (var stream = new FileStream(clientSecretsPath, FileMode.Open, FileAccess.Read))
      {
        var clientSecrets = await GoogleClientSecrets.FromStreamAsync(stream);

        var initializer = new Google.Apis.Auth.OAuth2.Flows.GoogleAuthorizationCodeFlow.Initializer
        {
          ClientSecrets = clientSecrets.Secrets,
          Scopes = new[] { DriveService.Scope.Drive }
        };

        var flow = new Google.Apis.Auth.OAuth2.Flows.GoogleAuthorizationCodeFlow(initializer);

        var token = new Google.Apis.Auth.OAuth2.Responses.TokenResponse 
        { 
          RefreshToken = refreshToken,
          ExpiresInSeconds = 3600
        };

        var credential = new UserCredential(flow, "user", token);
        return credential;
      }
    }

    /// <summary>
    /// Inicializa el servicio de Google Drive usando Refresh Token
    /// </summary>
    private async Task InitializeDriveServiceAsync()
    {
      if (_driveService != null)
        return;

      try
      {
        if (string.IsNullOrEmpty(_config.ClientSecretsPath))
          throw new InvalidOperationException("ClientSecretsPath no está configurado para Google Drive");

        if (!File.Exists(_config.ClientSecretsPath))
          throw new FileNotFoundException($"Archivo de credenciales OAuth2 no encontrado: {_config.ClientSecretsPath}");

        if (string.IsNullOrEmpty(_config.RefreshToken))
          throw new InvalidOperationException("RefreshToken no está configurado. Necesitas ejecutar el script de autenticación primero.");

        // Crear credencial usando el Refresh Token
        var credential = await CreateCredentialFromRefreshTokenAsync(_config.ClientSecretsPath, _config.RefreshToken);

        // Crear el servicio de Drive
        _driveService = new DriveService(new BaseClientService.Initializer()
        {
          HttpClientInitializer = credential,
          ApplicationName = "CondominioAPI",
          GZipEnabled = false
        });

        // Obtener o crear la carpeta CondoAdmin
        await ObtenerOCrearCarpetaAdminAsync();

        Log.Information("Servicio de Google Drive inicializado exitosamente");
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error al inicializar el servicio de Google Drive");
        throw;
      }
    }

    /// <summary>
    /// Obtiene o crea la carpeta CondoAdmin
    /// </summary>
    private async Task ObtenerOCrearCarpetaAdminAsync()
    {
      try
      {
        string folderName = "CondoAdmin";

        // Buscar si la carpeta ya existe
        var listRequest = _driveService.Files.List();
        listRequest.Q = $"name='{folderName}' and mimeType='application/vnd.google-apps.folder' and trashed=false";
        listRequest.Spaces = "drive";
        listRequest.Fields = "files(id, name)";
        listRequest.PageSize = 1;

        var files = await listRequest.ExecuteAsync();

        if (files.Files != null && files.Files.Count > 0)
        {
          _rootFolderId = files.Files[0].Id;
          Log.Information("Carpeta CondoAdmin encontrada: {0}", _rootFolderId);
        }
        else
        {
          // Crear la carpeta
          var fileMetadata = new Google.Apis.Drive.v3.Data.File()
          {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder"
          };

          var createRequest = _driveService.Files.Create(fileMetadata);
          createRequest.Fields = "id";
          var folder = await createRequest.ExecuteAsync();
          _rootFolderId = folder.Id;
          Log.Information("Carpeta CondoAdmin creada: {0}", _rootFolderId);
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error al obtener o crear la carpeta CondoAdmin");
        throw;
      }
    }

    /// <summary>
    /// Sube un archivo a Google Drive
    /// </summary>
    public async Task<CloudStorageResult> UploadFileAsync(string localFilePath, string remoteFileName)
    {
      var result = new CloudStorageResult();

      try
      {
        if (!File.Exists(localFilePath))
        {
          result.Success = false;
          result.ErrorMessage = $"Archivo local no encontrado: {localFilePath}";
          Log.Warning("Archivo no encontrado para subir a Google Drive: {0}", localFilePath);
          return result;
        }

        await InitializeDriveServiceAsync();

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
          Name = remoteFileName,
          Parents = new List<string> { _rootFolderId }
        };

        using (var stream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
        {
          var request = _driveService.Files.Create(fileMetadata, stream, "application/pdf");
          request.Fields = "id, webViewLink, webContentLink";

          var uploadProgress = await request.UploadAsync();

          if (uploadProgress.Status == Google.Apis.Upload.UploadStatus.Completed)
          {
            var uploadedFile = request.ResponseBody;
            result.Success = true;
            result.FileId = uploadedFile.Id;
            result.FileUrl = uploadedFile.WebViewLink;

            Log.Information("Archivo subido exitosamente a Google Drive: {0} (ID: {1})", 
              remoteFileName, uploadedFile.Id);
          }
          else
          {
            result.Success = false;
            result.ErrorMessage = $"Error al subir archivo: {uploadProgress.Exception?.Message}";
            Log.Error("Error en la carga a Google Drive: {0}", uploadProgress.Exception?.Message);
          }
        }

        return result;
      }
      catch (Exception ex)
      {
        result.Success = false;
        result.ErrorMessage = ex.Message;
        Log.Error(ex, "Excepción al subir archivo a Google Drive");
        return result;
      }
    }

    /// <summary>
    /// Elimina un archivo de Google Drive
    /// </summary>
    public async Task<bool> DeleteFileAsync(string fileId)
    {
      try
      {
        if (string.IsNullOrEmpty(fileId))
          return false;

        await InitializeDriveServiceAsync();

        var request = _driveService.Files.Delete(fileId);
        await request.ExecuteAsync();

        Log.Information("Archivo eliminado de Google Drive: {0}", fileId);
        return true;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error al eliminar archivo de Google Drive: {0}", fileId);
        return false;
      }
    }

    /// <summary>
    /// Obtiene información del archivo en Google Drive
    /// </summary>
    public async Task<CloudStorageFileInfo> GetFileInfoAsync(string fileId)
    {
      try
      {
        if (string.IsNullOrEmpty(fileId))
          return null;

        await InitializeDriveServiceAsync();

        var request = _driveService.Files.Get(fileId);
        request.Fields = "id, name, size, createdTime, mimeType, webViewLink";

        var file = await request.ExecuteAsync();

        return new CloudStorageFileInfo
        {
          FileId = file.Id,
          FileName = file.Name,
          FileUrl = file.WebViewLink,
          SizeInBytes = file.Size ?? 0,
          CreatedAt = file.CreatedTime ?? DateTime.UtcNow,
          MimeType = file.MimeType
        };
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error al obtener información del archivo de Google Drive: {0}", fileId);
        return null;
      }
    }

    /// <summary>
    /// Verifica si Google Drive está configurado correctamente
    /// </summary>
    public async Task<bool> IsConfiguredAsync()
    {
      try
      {
        if (string.IsNullOrEmpty(_config.ClientSecretsPath))
          return false;

        if (!File.Exists(_config.ClientSecretsPath))
          return false;

        if (string.IsNullOrEmpty(_config.RefreshToken))
          return false;

        // Intentar inicializar para validar credenciales
        await InitializeDriveServiceAsync();
        return _driveService != null && !string.IsNullOrEmpty(_rootFolderId);
      }
      catch (Exception ex)
      {
        Log.Warning(ex, "Google Drive no está correctamente configurado");
        return false;
      }
    }
  }
}

