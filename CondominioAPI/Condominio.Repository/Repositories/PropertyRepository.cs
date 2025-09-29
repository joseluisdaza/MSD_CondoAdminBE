using Condominio.Data.MySql.Models;

namespace Condominio.Repository.Repositories
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        public PropertyRepository(CondominioContext context) : base(context) { }
        // Add entity-specific methods if needed
    }
}
