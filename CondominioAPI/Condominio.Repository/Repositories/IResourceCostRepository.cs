using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IResourceCostRepository : IRepository<ResourceCost>
  {
    Task<IEnumerable<ResourceCost>> GetByResourceIdAsync(int resourceId, bool includeFinalized = false);
    Task<ResourceCost?> GetCurrentResourceCostAsync(int resourceId);
  }
}
