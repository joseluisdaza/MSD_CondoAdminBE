using Condominio.Data.Mysql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public class IncidentCostRepository : Repository<IncidentCost>, IIncidentCostRepository
  {
    public IncidentCostRepository(CondominioContext context) : base(context) { }
    // Add entity-specific methods if needed
  }
}
