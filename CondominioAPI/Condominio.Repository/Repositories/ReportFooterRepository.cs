using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
  public class ReportFooterRepository : Repository<ReportFooter>, IReportFooterRepository
  {
    public ReportFooterRepository(CondominioContext context) : base(context) { }

    public async Task<IEnumerable<ReportFooter>> GetByReportIdAsync(int reportId)
    {
      return await _context.ReportFooters
        .Where(rf => rf.ReportId == reportId)
        .Include(rf => rf.Style)
        .ToListAsync();
    }

    public async Task<IEnumerable<ReportFooter>> GetByReportIdOrderedAsync(int reportId)
    {
      return await _context.ReportFooters
        .Where(rf => rf.ReportId == reportId)
        .OrderBy(rf => rf.DisplayOrder)
        .Include(rf => rf.Style)
        .ToListAsync();
    }
  }
}
