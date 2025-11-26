using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Security.Claims;

namespace Condominio.Utils.Logs
{
    public class Logger
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _serilogLogger;

        public Logger(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _serilogLogger = Log.Logger;
        }

        /// <summary>
        /// Registra información general
        /// </summary>
        public void LogInformation(string message, params object[] args)
        {
            _serilogLogger.Information(message, args);
        }

        /// <summary>
        /// Registra una advertencia
        /// </summary>
        public void LogWarning(string message, params object[] args)
        {
            _serilogLogger.Warning(message, args);
        }

        /// <summary>
        /// Registra un error
        /// </summary>
        public void LogError(Exception exception, string message, params object[] args)
        {
            _serilogLogger.Error(exception, message, args);
        }

        /// <summary>
        /// Registra un error sin excepción
        /// </summary>
        public void LogError(string message, params object[] args)
        {
            _serilogLogger.Error(message, args);
        }

        /// <summary>
        /// Registra debug information
        /// </summary>
        public void LogDebug(string message, params object[] args)
        {
            _serilogLogger.Debug(message, args);
        }

        /// <summary>
        /// Registra información crítica
        /// </summary>
        public void LogFatal(Exception exception, string message, params object[] args)
        {
            _serilogLogger.Fatal(exception, message, args);
        }

        /// <summary>
        /// Registra una acción de auditoría en la base de datos
        /// </summary>
        public async Task LogAuditAsync(int userId, string action, string message, string? tableName = null)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<CondominioContext>();

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    Action = action,
                    TableName = tableName,
                    Message = message,
                    Timestamp = DateTime.Now
                };

                context.AuditLogs.Add(auditLog);
                await context.SaveChangesAsync();

                // También log en Serilog
                _serilogLogger.Information("AUDIT: User {UserId} performed {Action} on {TableName}: {Message}",
                    userId, action, tableName ?? "N/A", message);
            }
            catch (Exception ex)
            {
                // Si falla el log de auditoría, al menos registrar en Serilog
                _serilogLogger.Error(ex, "Failed to save audit log for user {UserId}, action {Action}: {Message}",
                    userId, action, message);
            }
        }

        /// <summary>
        /// Registra una acción de auditoría usando el usuario del contexto HTTP
        /// </summary>
        public async Task LogAuditAsync(ClaimsPrincipal user, string action, string message, string? tableName = null)
        {
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                await LogAuditAsync(userId, action, message, tableName);
            }
            else
            {
                // Si no se puede obtener el usuario, registrar como usuario anónimo o sistema
                await LogAuditAsync(0, action, $"[ANONYMOUS/SYSTEM] {message}", tableName);
            }
        }

        /// <summary>
        /// Registra acciones CRUD específicas
        /// </summary>
        public async Task LogCreateAsync(int userId, string tableName, object entityData, int? entityId = null)
        {
            var message = entityId.HasValue
                ? $"Created record with ID {entityId} in {tableName}: {System.Text.Json.JsonSerializer.Serialize(entityData)}"
                : $"Created record in {tableName}: {System.Text.Json.JsonSerializer.Serialize(entityData)}";

            await LogAuditAsync(userId, "CREATE", message, tableName);
        }

        public async Task LogUpdateAsync(int userId, string tableName, int entityId, object? oldData = null, object? newData = null)
        {
            var message = $"Updated record with ID {entityId} in {tableName}";
            if (oldData != null && newData != null)
            {
                message += $". Old: {System.Text.Json.JsonSerializer.Serialize(oldData)}, New: {System.Text.Json.JsonSerializer.Serialize(newData)}";
            }

            await LogAuditAsync(userId, "UPDATE", message, tableName);
        }

        public async Task LogDeleteAsync(int userId, string tableName, int entityId, object? deletedData = null)
        {
            var message = $"Deleted record with ID {entityId} from {tableName}";
            if (deletedData != null)
            {
                message += $". Data: {System.Text.Json.JsonSerializer.Serialize(deletedData)}";
            }

            await LogAuditAsync(userId, "DELETE", message, tableName);
        }

        /// <summary>
        /// Registra acciones de autenticación
        /// </summary>
        public async Task LogLoginAsync(int userId, string username, string? ipAddress = null)
        {
            var message = $"User {username} logged in successfully";
            if (!string.IsNullOrEmpty(ipAddress))
            {
                message += $" from IP: {ipAddress}";
            }

            await LogAuditAsync(userId, "LOGIN", message);
        }

        public async Task LogLogoutAsync(int userId, string username)
        {
            await LogAuditAsync(userId, "LOGOUT", $"User {username} logged out");
        }

        public async Task LogFailedLoginAsync(string username, string? ipAddress = null, string? reason = null)
        {
            var message = $"Failed login attempt for user {username}";
            if (!string.IsNullOrEmpty(reason))
            {
                message += $". Reason: {reason}";
            }
            if (!string.IsNullOrEmpty(ipAddress))
            {
                message += $" from IP: {ipAddress}";
            }

            // Para intentos fallidos, usar userId 0 (sistema)
            await LogAuditAsync(0, "FAILED_LOGIN", message);
        }

        /// <summary>
        /// Registra acciones de acceso y permisos
        /// </summary>
        public async Task LogAccessDeniedAsync(int userId, string resource, string action)
        {
            await LogAuditAsync(userId, "ACCESS_DENIED", $"Access denied to {resource} for action {action}");
        }

        public async Task LogPermissionChangeAsync(int userId, int targetUserId, string permission, bool granted)
        {
            var action = granted ? "granted" : "revoked";
            await LogAuditAsync(userId, "PERMISSION_CHANGE",
                $"Permission '{permission}' {action} for user ID {targetUserId}");
        }

        /// <summary>
        /// Registra errores del sistema
        /// </summary>
        public async Task LogSystemErrorAsync(Exception exception, string context, int? userId = null)
        {
            var message = $"System error in {context}: {exception.Message}";
            await LogAuditAsync(userId ?? 0, "SYSTEM_ERROR", message);

            // También registrar en Serilog para stack trace completo
            _serilogLogger.Error(exception, "System error in {Context}", context);
        }

        /// <summary>
        /// Registra configuraciones del sistema
        /// </summary>
        public async Task LogConfigurationChangeAsync(int userId, string setting, object? oldValue, object newValue)
        {
            var message = $"Configuration changed - Setting: {setting}, Old: {oldValue}, New: {newValue}";
            await LogAuditAsync(userId, "CONFIG_CHANGE", message);
        }
    }
}
