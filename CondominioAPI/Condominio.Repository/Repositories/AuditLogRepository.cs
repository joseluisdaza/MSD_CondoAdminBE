using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(CondominioContext context) : base(context)
    {
    }

    public async Task<AuditLog?> GetByIdAsync(int id)
    {
        return await _context.AuditLogs
            .Include(al => al.User)
            .FirstOrDefaultAsync(al => al.Id == id);
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .Include(al => al.User)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId)
    {
        return await _context.AuditLogs
            .Include(al => al.User)
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action)
    {
        return await _context.AuditLogs
            .Include(al => al.User)
            .Where(al => al.Action == action)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByTableNameAsync(string tableName)
    {
        return await _context.AuditLogs
            .Include(al => al.User)
            .Where(al => al.TableName == tableName)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.AuditLogs
            .Include(al => al.User)
            .Where(al => al.Timestamp >= startDate && al.Timestamp <= endDate)
            .OrderByDescending(al => al.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 100)
    {
        return await _context.AuditLogs
            .Include(al => al.User)
            .OrderByDescending(al => al.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}