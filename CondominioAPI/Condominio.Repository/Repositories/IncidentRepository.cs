using Condominio.Data.Mysql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public class IncidentRepository : Repository<Incident>, IIncidentRepository
  {
    public IncidentRepository(CondominioContext context) : base(context) { }
    // Add entity-specific methods if needed
  }
}
