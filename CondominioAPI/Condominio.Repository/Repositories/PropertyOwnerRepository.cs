using Condominio.Data.MySql.Models;

namespace Condominio.Repository.Repositories
{
    public class PropertyOwnerRepository : Repository<PropertyOwner>, IPropertyOwnerRepository
    {
        public PropertyOwnerRepository(CondominioContext context) : base(context) { }
        // Add entity-specific methods if needed
    }
}
