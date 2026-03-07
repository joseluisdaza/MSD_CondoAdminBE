using Condominio.Data.MySql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public class ResourceBookingRepository : Repository<ResourceBooking>, IResourceBookingRepository
  {
    public ResourceBookingRepository(CondominioContext context) : base(context) { }
    // Add entity-specific methods if needed
  }
}
