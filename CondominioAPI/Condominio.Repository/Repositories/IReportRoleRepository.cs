using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IReportRoleRepository : IRepository<ReportRole>
  {
    Task<IEnumerable<ReportRole>> GetByReportIdAsync(int reportId);
    Task<ReportRole?> GetByReportIdAndRoleIdAsync(int reportId, int roleId);
    Task DeleteByReportIdAndRoleIdAsync(int reportId, int roleId);
  }
}
