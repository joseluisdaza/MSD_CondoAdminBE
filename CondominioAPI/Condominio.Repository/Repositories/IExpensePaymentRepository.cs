using Condominio.Models;

namespace Condominio.Repository.Repositories;

public interface IExpensePaymentRepository : IRepository<ExpensePayment>
{
    Task<IEnumerable<ExpensePayment>> GetByExpenseIdAsync(int expenseId);
    Task<IEnumerable<ExpensePayment>> GetByPaymentIdAsync(int paymentId);
    Task<ExpensePayment?> GetByExpenseAndPaymentIdAsync(int expenseId, int paymentId);
}