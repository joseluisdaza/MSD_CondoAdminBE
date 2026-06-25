using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
  public class ReportHeaderRepository : Repository<ReportHeader>, IReportHeaderRepository
  {
    public ReportHeaderRepository(CondominioContext context) : base(context) { }

    public async Task<IEnumerable<ReportHeader>> GetByReportIdAsync(int reportId)
    {
      return await _context.ReportHeaders
        .Where(rh => rh.ReportId == reportId)
        .Include(rh => rh.Style)
        .ToListAsync();
    }

    public async Task<IEnumerable<ReportHeader>> GetByReportIdOrderedAsync(int reportId)
    {
      return await _context.ReportHeaders
        .Where(rh => rh.ReportId == reportId)
        .OrderBy(rh => rh.DisplayOrder)
        .Include(rh => rh.Style)
        .ToListAsync();
    }
  }
}
