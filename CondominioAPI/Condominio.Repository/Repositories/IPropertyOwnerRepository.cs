using Condominio.Data.MySql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
    public interface IPropertyOwnerRepository : IRepository<PropertyOwner>
    {
        Task<PropertyOwner?> GetByPropertyAndUserIdAsync(int propertyId, int userId);
        Task<IEnumerable<PropertyOwner>> GetByPropertyIdAsync(int propertyId, bool includeFinalized = false);
        Task<IEnumerable<PropertyOwner>> GetByUserIdAsync(int userId, bool includeFinalized = false);
        Task<IEnumerable<PropertyOwner>> GetAllWithRelationsAsync(bool includeFinalized = false);
        Task<IEnumerable<PropertyOwner>> GetFilteredAsync(int? propertyId = null, int? userId = null, bool includeFinalized = false);
    }
}
