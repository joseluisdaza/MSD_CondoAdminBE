using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories;

public class ServiceTypeRepository : Repository<ServiceType>, IServiceTypeRepository
{
    public ServiceTypeRepository(CondominioContext context) : base(context)
    {
    }

    public async Task<ServiceType?> GetByIdAsync(int id)
    {
        return await _context.ServiceTypes
            .Include(st => st.ServiceExpenses)
            .FirstOrDefaultAsync(st => st.Id == id);
    }

    public async Task<IEnumerable<ServiceType>> GetAllAsync()
    {
        return await _context.ServiceTypes
            .Include(st => st.ServiceExpenses)
            .ToListAsync();
    }

    public async Task<ServiceType?> GetByServiceNameAsync(string serviceName)
    {
        return await _context.ServiceTypes
            .Include(st => st.ServiceExpenses)
            .FirstOrDefaultAsync(st => st.ServiceName == serviceName);
    }
}