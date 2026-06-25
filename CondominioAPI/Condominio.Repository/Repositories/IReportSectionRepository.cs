using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IReportSectionRepository : IRepository<ReportSection>
  {
    Task<IEnumerable<ReportSection>> GetByReportIdAsync(int reportId);
    Task<IEnumerable<ReportSection>> GetByReportIdOrderedAsync(int reportId);
  }
}
