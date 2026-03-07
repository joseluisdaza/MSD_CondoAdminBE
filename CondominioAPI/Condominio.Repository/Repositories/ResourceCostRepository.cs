using Condominio.Data.MySql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public class ResourceCostRepository : Repository<ResourceCost>, IResourceCostRepository
  {
    public ResourceCostRepository(CondominioContext context) : base(context) { }
    // Add entity-specific methods if needed
  }
}
