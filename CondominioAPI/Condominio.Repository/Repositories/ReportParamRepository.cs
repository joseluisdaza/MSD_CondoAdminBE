using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
  public class ReportParamRepository : Repository<ReportParam>, IReportParamRepository
  {
    public ReportParamRepository(CondominioContext context) : base(context) { }

    public async Task<IEnumerable<ReportParam>> GetByReportIdAsync(int reportId)
    {
      return await _context.ReportParams
        .Where(rp => rp.ReportId == reportId && rp.EndDate == null)
        .OrderBy(rp => rp.ParamName)
        .ToListAsync();
    }
  }
}
