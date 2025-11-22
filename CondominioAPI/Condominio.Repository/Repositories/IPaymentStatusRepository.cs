using Condominio.Models;

namespace Condominio.Repository.Repositories;

public interface IPaymentStatusRepository : IRepository<PaymentStatus>
{
    Task<PaymentStatus?> GetByIdAsync(int id);
    Task<IEnumerable<PaymentStatus>> GetAllAsync();
}