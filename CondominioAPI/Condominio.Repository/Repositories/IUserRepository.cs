using Condominio.Data.MySql.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Condominio.Repository.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        // Add user-specific methods here if needed
        Task<User?> GetByUserNameAsync(string userName);
    }
}
