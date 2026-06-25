using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IReportHeaderRepository : IRepository<ReportHeader>
  {
    Task<IEnumerable<ReportHeader>> GetByReportIdAsync(int reportId);
    Task<IEnumerable<ReportHeader>> GetByReportIdOrderedAsync(int reportId);
  }
}
