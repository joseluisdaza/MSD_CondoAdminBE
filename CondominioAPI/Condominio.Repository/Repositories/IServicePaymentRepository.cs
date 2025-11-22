using Condominio.Models;

namespace Condominio.Repository.Repositories;

public interface IServicePaymentRepository : IRepository<ServicePayment>
{
    Task<ServicePayment?> GetByIdAsync(int id);
    Task<IEnumerable<ServicePayment>> GetAllAsync();
    Task<IEnumerable<ServicePayment>> GetByStatusIdAsync(int statusId);
    Task<ServicePayment?> GetByReceiveNumberAsync(string receiveNumber);
    Task<IEnumerable<ServicePayment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}