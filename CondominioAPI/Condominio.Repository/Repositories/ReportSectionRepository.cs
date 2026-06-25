using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
  public class ReportSectionRepository : Repository<ReportSection>, IReportSectionRepository
  {
    public ReportSectionRepository(CondominioContext context) : base(context) { }

    public async Task<IEnumerable<ReportSection>> GetByReportIdAsync(int reportId)
    {
      return await _context.ReportSections
        .Where(rs => rs.ReportId == reportId)
        .Include(rs => rs.Style)
        .Include(rs => rs.HeaderStyle)
        .ToListAsync();
    }

    public async Task<IEnumerable<ReportSection>> GetByReportIdOrderedAsync(int reportId)
    {
      return await _context.ReportSections
        .Where(rs => rs.ReportId == reportId)
        .OrderBy(rs => rs.DisplayOrder)
        .Include(rs => rs.Style)
        .Include(rs => rs.HeaderStyle)
        .ToListAsync();
    }
  }
}
