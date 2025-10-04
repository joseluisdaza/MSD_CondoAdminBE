using Condominio.Models;

namespace Condominio.Repository.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        // Add user-specific methods here if needed
        Task<User?> GetByUserNameAsync(string userName);

        Task<User?> GetByLoginAsync(string login);
    }
}
