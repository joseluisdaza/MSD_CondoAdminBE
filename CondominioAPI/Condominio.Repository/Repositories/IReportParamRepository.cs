using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IReportParamRepository : IRepository<ReportParam>
  {
    Task<IEnumerable<ReportParam>> GetByReportIdAsync(int reportId);
  }
}
