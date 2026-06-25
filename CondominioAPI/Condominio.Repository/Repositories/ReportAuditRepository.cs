using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
  public class ReportAuditRepository : Repository<ReportAudit>, IReportAuditRepository
  {
    public ReportAuditRepository(CondominioContext context) : base(context) { }

    public async Task<IEnumerable<ReportAudit>> GetByReportIdAsync(int reportId)
    {
      return await _context.ReportAudits
        .Where(ra => ra.ReportId == reportId)
        .Include(ra => ra.User)
        .OrderByDescending(ra => ra.ExecutionDate)
        .ToListAsync();
    }

    public async Task<IEnumerable<ReportAudit>> GetByUserIdAsync(int userId)
    {
      return await _context.ReportAudits
        .Where(ra => ra.UserId == userId)
        .Include(ra => ra.Report)
        .OrderByDescending(ra => ra.ExecutionDate)
        .ToListAsync();
    }

    public async Task<IEnumerable<ReportAudit>> GetByReportIdAndDateRangeAsync(int reportId, DateTime startDate, DateTime endDate)
    {
      return await _context.ReportAudits
        .Where(ra => ra.ReportId == reportId && ra.ExecutionDate >= startDate && ra.ExecutionDate <= endDate)
        .Include(ra => ra.User)
        .OrderByDescending(ra => ra.ExecutionDate)
        .ToListAsync();
    }
  }
}
