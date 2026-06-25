using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IReportFooterRepository : IRepository<ReportFooter>
  {
    Task<IEnumerable<ReportFooter>> GetByReportIdAsync(int reportId);
    Task<IEnumerable<ReportFooter>> GetByReportIdOrderedAsync(int reportId);
  }
}
