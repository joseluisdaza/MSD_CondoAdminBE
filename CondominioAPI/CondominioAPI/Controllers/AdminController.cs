using CondominioAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CondominioAPI.Controllers
{
  /// <summary>
  /// Controlador de administración para tareas de configuración
  /// ⚠️ IMPORTANTE: Este controlador solo debe estar disponible en desarrollo
  /// En producción, debe estar deshabilitado o protegido con autenticación estricta
  /// </summary>
  [ApiController]
  [Route("api/[controller]")]
  public class AdminController : ControllerBase
  {
    /// <summary>
    /// Obtiene el Refresh Token para Google Drive
    /// 
    /// Este endpoint debe ejecutarse UNA sola vez desde una máquina con navegador.
    /// Abrirá el navegador para que autorices la aplicación con tu cuenta de Gmail.
    /// Luego devuelve el Refresh Token que debes guardar en .env
    /// 
    /// ⚠️ SOLO DISPONIBLE EN DESARROLLO - Se debe desactivar en producción
    /// </summary>
    /// <remarks>
    /// Ejemplo de uso:
    /// GET /api/admin/google-drive-auth
    /// 
    /// Respuesta exitosa (200):
    /// {
    ///   "success": true,
    ///   "refreshToken": "1//abc123...",
    ///   "message": "Copia el refreshToken y agrégalo a tu .env como GOOGLE_DRIVE_REFRESH_TOKEN"
    /// }
    /// </remarks>
    [HttpGet("google-drive-auth")]
    [ApiExplorerSettings(IgnoreApi = true)]  // Oculta del Swagger
    public async Task<IActionResult> GetGoogleDriveAuthToken()
    {
      try
      {
        // ⚠️ Verificar que estamos en desarrollo
        if (!this.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
          Log.Warning("Intento de acceso a endpoint de autenticación de Google Drive en producción");
          return Forbid("Este endpoint solo está disponible en desarrollo");
        }

        var credentialsPath = Environment.GetEnvironmentVariable("GOOGLE_DRIVE_CLIENT_SECRETS_PATH");

        if (string.IsNullOrEmpty(credentialsPath))
        {
          return BadRequest(new
          {
            success = false,
            error = "GOOGLE_DRIVE_CLIENT_SECRETS_PATH no está configurado"
          });
        }

        if (!System.IO.File.Exists(credentialsPath))
        {
          return BadRequest(new
          {
            success = false,
            error = $"Archivo credentials.json no encontrado en: {credentialsPath}"
          });
        }

        Log.Information("Iniciando proceso de autenticación de Google Drive");

        var refreshToken = await GoogleDriveAuthSetup.GetRefreshTokenAsync(credentialsPath);

        return Ok(new
        {
          success = true,
          refreshToken = refreshToken,
          message = "✅ Copia el refreshToken y agrégalo a tu .env como: GOOGLE_DRIVE_REFRESH_TOKEN=<token>",
          instructions = new[]
          {
            "1. Copia el refreshToken anterior",
            "2. Abre CondominioAPI/.env",
            "3. Agrega: GOOGLE_DRIVE_REFRESH_TOKEN={refreshToken}",
            "4. Guarda el archivo",
            "5. Reinicia la aplicación"
          }
        });
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error al obtener Refresh Token de Google Drive");
        return StatusCode(500, new
        {
          success = false,
          error = ex.Message,
          details = ex.InnerException?.Message
        });
      }
    }
  }
}
