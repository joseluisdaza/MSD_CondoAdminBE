using Condominio.Data.Mysql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public class ResourceRepository : Repository<Resource>, IResourceRepository
  {
    public ResourceRepository(CondominioContext context) : base(context) { }
    // Add entity-specific methods if needed
  }
}
