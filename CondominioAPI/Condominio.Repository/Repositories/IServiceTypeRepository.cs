using Condominio.Models;

namespace Condominio.Repository.Repositories;

public interface IServiceTypeRepository : IRepository<ServiceType>
{
    Task<ServiceType?> GetByIdAsync(int id);
    Task<IEnumerable<ServiceType>> GetAllAsync();
    Task<ServiceType?> GetByServiceNameAsync(string serviceName);
}