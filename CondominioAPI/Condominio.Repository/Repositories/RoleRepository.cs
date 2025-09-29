using Condominio.Data.MySql.Models;

namespace Condominio.Repository.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(CondominioContext context) : base(context) { }
        // Add entity-specific methods if needed
    }
}
