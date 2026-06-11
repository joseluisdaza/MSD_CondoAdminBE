using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
  public class ReportRoleRepository : Repository<ReportRole>, IReportRoleRepository
  {
    public ReportRoleRepository(CondominioContext context) : base(context) { }

    public async Task<IEnumerable<ReportRole>> GetByReportIdAsync(int reportId)
    {
      return await _context.ReportRoles
        .Include(rr => rr.Role)
        .Where(rr => rr.ReportId == reportId)
        .ToListAsync();
    }

    public async Task<ReportRole?> GetByReportIdAndRoleIdAsync(int reportId, int roleId)
    {
      return await _context.ReportRoles
        .FirstOrDefaultAsync(rr => rr.ReportId == reportId && rr.RoleId == roleId);
    }

    public async Task DeleteByReportIdAndRoleIdAsync(int reportId, int roleId)
    {
      var reportRole = await GetByReportIdAndRoleIdAsync(reportId, roleId);
      if (reportRole != null)
      {
        _context.ReportRoles.Remove(reportRole);
        await _context.SaveChangesAsync();
      }
    }
  }
}
