using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
  public class ResourceCostRepository : Repository<ResourceCost>, IResourceCostRepository
  {
    public ResourceCostRepository(CondominioContext context) : base(context) { }

    public async Task<IEnumerable<ResourceCost>> GetByResourceIdAsync(int resourceId, bool includeFinalized = false)
    {
      IQueryable<ResourceCost> query = _context.ResourceCosts
        .Include(rc => rc.Resource)
        .Where(rc => rc.ResourceId == resourceId);

      if (!includeFinalized)
      {
        query = query.Where(rc => rc.EndDate == null);
      }

      return await query.OrderByDescending(rc => rc.StartDate).ToListAsync();
    }

    public async Task<ResourceCost?> GetCurrentResourceCostAsync(int resourceId)
    {
      return await _context.ResourceCosts
        .Include(rc => rc.Resource)
        .Where(rc => rc.ResourceId == resourceId && rc.EndDate == null)
        .OrderByDescending(rc => rc.StartDate)
        .FirstOrDefaultAsync();
    }
  }
}
