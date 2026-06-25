using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IReportAuditRepository : IRepository<ReportAudit>
  {
    Task<IEnumerable<ReportAudit>> GetByReportIdAsync(int reportId);
    Task<IEnumerable<ReportAudit>> GetByUserIdAsync(int userId);
    Task<IEnumerable<ReportAudit>> GetByReportIdAndDateRangeAsync(int reportId, DateTime startDate, DateTime endDate);
  }
}
