using Condominio.Data.MySql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public class IncidentTypeRepository : Repository<IncidentType>, IIncidentTypeRepository
  {
    public IncidentTypeRepository(CondominioContext context) : base(context) { }
    // Add entity-specific methods if needed
  }
}
