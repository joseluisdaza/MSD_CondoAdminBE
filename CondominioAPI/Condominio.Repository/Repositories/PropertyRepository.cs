using Condominio.Data.Mysql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        public PropertyRepository(CondominioContext context) : base(context) { }
        // Add entity-specific methods if needed
    }
}
