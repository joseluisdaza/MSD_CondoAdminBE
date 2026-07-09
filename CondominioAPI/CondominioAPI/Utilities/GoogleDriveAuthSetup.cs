using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CondominioAPI.Utilities
{
  /// <summary>
  /// Utilidad para obtener un Refresh Token de Google Drive
  /// Este script debe ejecutarse UNA sola vez desde una máquina con navegador
  /// El refresh token se usa indefinidamente en el API
  /// </summary>
  public class GoogleDriveAuthSetup
  {
    /// <summary>
    /// Obtiene el Refresh Token para Google Drive
    /// Ejecutar solo UNA vez e incluir el token en el archivo .env
    /// </summary>
    public static async Task<string> GetRefreshTokenAsync(string credentialsJsonPath)
    {
      try
      {
        if (!File.Exists(credentialsJsonPath))
          throw new FileNotFoundException($"Archivo credentials.json no encontrado: {credentialsJsonPath}");

        Console.WriteLine("📱 Obteniendo Refresh Token para Google Drive...");
        Console.WriteLine("Se abrirá una ventana del navegador. Debes autorizar la aplicación.");
        Console.WriteLine();

        UserCredential credential;

        using (var stream = new FileStream(credentialsJsonPath, FileMode.Open, FileAccess.Read))
        {
          // Crear directorio temporal para guardar token
          string tokenPath = Path.Combine(Path.GetTempPath(), "condominio_google_auth");
          Directory.CreateDirectory(tokenPath);

          credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            new[] { DriveService.Scope.Drive },
            "user",
            CancellationToken.None,
            new FileDataStore(tokenPath, true));
        }

        // Obtener el refresh token
        string refreshToken = credential.Token.RefreshToken;

        if (string.IsNullOrEmpty(refreshToken))
          throw new Exception("No se pudo obtener el Refresh Token. Por favor intenta de nuevo.");

        Console.WriteLine();
        Console.WriteLine("✅ ¡Éxito! Se obtuvo el Refresh Token:");
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine(refreshToken);
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("📋 Instrucciones:");
        Console.WriteLine("1. Copia el token anterior");
        Console.WriteLine("2. Abre tu archivo .env en: CondominioAPI/.env");
        Console.WriteLine("3. Agrega esta línea:");
        Console.WriteLine($"   GOOGLE_DRIVE_REFRESH_TOKEN={refreshToken}");
        Console.WriteLine("4. Guarda el archivo");
        Console.WriteLine("5. Reinicia la aplicación");
        Console.WriteLine();
        Console.WriteLine("🔒 IMPORTANTE: Nunca compartas este token. Es como tu contraseña.");
        Console.WriteLine();

        return refreshToken;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"❌ Error: {ex.Message}");
        throw;
      }
    }
  }
}
