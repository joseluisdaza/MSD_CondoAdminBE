using Condominio.Data.MySql.Models;

namespace Condominio.Repository.Repositories
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(CondominioContext context) : base(context) { }
        // Add entity-specific methods if needed
    }
}
