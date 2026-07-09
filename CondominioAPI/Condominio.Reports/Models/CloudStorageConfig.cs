namespace Condominio.Reports.Models
{
  /// <summary>
  /// Configuración general de almacenamiento en la nube
  /// </summary>
  public class CloudStorageConfig
  {
    /// <summary>
    /// Proveedor a utilizar: "GoogleDrive", "OneDrive", "S3", etc.
    /// </summary>
    public string Provider { get; set; }

    /// <summary>
    /// Indica si el almacenamiento en la nube está habilitado
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Configuración específica de Google Drive
    /// </summary>
    public GoogleDriveConfig GoogleDrive { get; set; }

    /// <summary>
    /// Configuración específica de OneDrive
    /// </summary>
    public OneDriveConfig OneDrive { get; set; }

    public CloudStorageConfig()
    {
      GoogleDrive = new GoogleDriveConfig();
      OneDrive = new OneDriveConfig();
    }
  }

  /// <summary>
  /// Configuración para Google Drive con OAuth2 personal y Refresh Token
  /// </summary>
  public class GoogleDriveConfig
  {
    /// <summary>
    /// Ruta al archivo credentials.json descargado de Google Cloud Console
    /// (Credenciales OAuth2 para aplicación de escritorio)
    /// </summary>
    public string ClientSecretsPath { get; set; }

    /// <summary>
    /// Refresh Token obtenido mediante el script de autenticación
    /// Se ejecuta solo UNA vez para obtener este token, luego se reutiliza indefinidamente
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// Timeout para operaciones con Google Drive (segundos)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
  }

  /// <summary>
  /// Configuración para OneDrive (futura extensión)
  /// </summary>
  public class OneDriveConfig
  {
    /// <summary>
    /// Client ID de la aplicación en Azure AD
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Client Secret de la aplicación en Azure AD
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Tenant ID en Azure AD
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// Ruta de la carpeta en OneDrive donde se guardarán los reportes
    /// </summary>
    public string ReportsFolderPath { get; set; }

    /// <summary>
    /// Timeout para operaciones con OneDrive (segundos)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
  }
}
