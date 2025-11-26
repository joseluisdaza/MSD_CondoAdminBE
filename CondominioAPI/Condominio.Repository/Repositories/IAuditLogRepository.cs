using Condominio.Models;

namespace Condominio.Repository.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<AuditLog?> GetByIdAsync(int id);
    Task<IEnumerable<AuditLog>> GetAllAsync();
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId);
    Task<IEnumerable<AuditLog>> GetByActionAsync(string action);
    Task<IEnumerable<AuditLog>> GetByTableNameAsync(string tableName);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 100);
}