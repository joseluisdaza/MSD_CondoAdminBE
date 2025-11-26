using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogsController(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        /// <summary>
        /// Obtiene todos los logs de auditoría (Solo Super Administradores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = AppRoles.Super)]
        public async Task<ActionResult<IEnumerable<AuditLogResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > AuditLogs > GetAll. User: {0}", User.Identity?.Name);
                
                var auditLogs = await _auditLogRepository.GetAllAsync();
                var auditLogResponses = auditLogs.Select(al => al.ToAuditLogResponse()).ToList();
                
                return Ok(auditLogResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all audit logs");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un log de auditoría por ID (Solo Super Administradores)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = AppRoles.Super)]
        public async Task<ActionResult<AuditLogResponse>> GetById(int id)
        {
            try
            {
                Log.Information("GET > AuditLogs > ById. User: {0}, Id: {1}", User.Identity?.Name, id);
                
                var auditLog = await _auditLogRepository.GetByIdAsync(id);
                if (auditLog == null)
                {
                    return NotFound($"Log de auditoría con ID {id} no encontrado");
                }

                return Ok(auditLog.ToAuditLogResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting audit log by ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene logs de auditoría por usuario (Solo Super Administradores)
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = AppRoles.Super)]
        public async Task<ActionResult<IEnumerable<AuditLogResponse>>> GetByUserId(int userId)
        {
            try
            {
                Log.Information("GET > AuditLogs > ByUserId. User: {0}, UserId: {1}", User.Identity?.Name, userId);
                
                var auditLogs = await _auditLogRepository.GetByUserIdAsync(userId);
                var auditLogResponses = auditLogs.Select(al => al.ToAuditLogResponse()).ToList();
                
                return Ok(auditLogResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting audit logs by user ID: {0}", userId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene logs de auditoría por acción (Solo Super Administradores)
        /// </summary>
        [HttpGet("action/{action}")]
        [Authorize(Roles = AppRoles.Super)]
        public async Task<ActionResult<IEnumerable<AuditLogResponse>>> GetByAction(string action)
        {
            try
            {
                Log.Information("GET > AuditLogs > ByAction. User: {0}, Action: {1}", User.Identity?.Name, action);
                
                var auditLogs = await _auditLogRepository.GetByActionAsync(action);
                var auditLogResponses = auditLogs.Select(al => al.ToAuditLogResponse()).ToList();
                
                return Ok(auditLogResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting audit logs by action: {0}", action);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene logs de auditoría por tabla (Solo Super Administradores)
        /// </summary>
        [HttpGet("table/{tableName}")]
        [Authorize(Roles = AppRoles.Super)]
        public async Task<ActionResult<IEnumerable<AuditLogResponse>>> GetByTableName(string tableName)
        {
            try
            {
                Log.Information("GET > AuditLogs > ByTableName. User: {0}, TableName: {1}", User.Identity?.Name, tableName);
                
                var auditLogs = await _auditLogRepository.GetByTableNameAsync(tableName);
                var auditLogResponses = auditLogs.Select(al => al.ToAuditLogResponse()).ToList();
                
                return Ok(auditLogResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting audit logs by table name: {0}", tableName);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene logs de auditoría por rango de fechas (Solo Super Administradores)
        /// </summary>
        [HttpGet("date-range")]
        [Authorize(Roles = AppRoles.Super)]
        public async Task<ActionResult<IEnumerable<AuditLogResponse>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                Log.Information("GET > AuditLogs > ByDateRange. User: {0}, StartDate: {1}, EndDate: {2}", 
                    User.Identity?.Name, startDate, endDate);
                
                if (startDate > endDate)
                {
                    return BadRequest("La fecha de inicio no puede ser mayor que la fecha de fin");
                }

                var auditLogs = await _auditLogRepository.GetByDateRangeAsync(startDate, endDate);
                var auditLogResponses = auditLogs.Select(al => al.ToAuditLogResponse()).ToList();
                
                return Ok(auditLogResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting audit logs by date range: {0} to {1}", startDate, endDate);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene los logs más recientes (Solo Super Administradores)
        /// </summary>
        [HttpGet("recent")]
        [Authorize(Roles = AppRoles.Super)]
        public async Task<ActionResult<IEnumerable<AuditLogResponse>>> GetRecentLogs([FromQuery] int count = 100)
        {
            try
            {
                Log.Information("GET > AuditLogs > Recent. User: {0}, Count: {1}", User.Identity?.Name, count);
                
                if (count <= 0 || count > 1000)
                {
                    return BadRequest("El número de registros debe estar entre 1 y 1000");
                }

                var auditLogs = await _auditLogRepository.GetRecentLogsAsync(count);
                var auditLogResponses = auditLogs.Select(al => al.ToAuditLogResponse()).ToList();
                
                return Ok(auditLogResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting recent audit logs");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}