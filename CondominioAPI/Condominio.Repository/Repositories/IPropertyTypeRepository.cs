using Condominio.Models;

namespace Condominio.Repository.Repositories
{
    public interface IPropertyTypeRepository : IRepository<PropertyType>
    {
        Task<PropertyType?> GetByTypeAsync(string type);
    }
}
