using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories;

public class ExpensePaymentRepository : Repository<ExpensePayment>, IExpensePaymentRepository
{
    public ExpensePaymentRepository(CondominioContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ExpensePayment>> GetByExpenseIdAsync(int expenseId)
    {
        return await _context.ExpensePayments
            .Include(ep => ep.Expense)
            .Include(ep => ep.Payment)
            .Where(ep => ep.ExpenseId == expenseId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ExpensePayment>> GetByPaymentIdAsync(int paymentId)
    {
        return await _context.ExpensePayments
            .Include(ep => ep.Expense)
            .Include(ep => ep.Payment)
            .Where(ep => ep.PaymentId == paymentId)
            .ToListAsync();
    }

    public async Task<ExpensePayment?> GetByExpenseAndPaymentIdAsync(int expenseId, int paymentId)
    {
        return await _context.ExpensePayments
            .Include(ep => ep.Expense)
            .Include(ep => ep.Payment)
            .FirstOrDefaultAsync(ep => ep.ExpenseId == expenseId && ep.PaymentId == paymentId);
    }
}