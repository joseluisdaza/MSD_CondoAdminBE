using Condominio.Models;

namespace Condominio.Repository.Repositories;

public interface IServiceExpensePaymentRepository : IRepository<ServiceExpensePayment>
{
    Task<IEnumerable<ServiceExpensePayment>> GetByServiceExpenseIdAsync(int serviceExpenseId);
    Task<IEnumerable<ServiceExpensePayment>> GetByPaymentIdAsync(int paymentId);
    Task<ServiceExpensePayment?> GetByServiceExpenseAndPaymentIdAsync(int serviceExpenseId, int paymentId);
}