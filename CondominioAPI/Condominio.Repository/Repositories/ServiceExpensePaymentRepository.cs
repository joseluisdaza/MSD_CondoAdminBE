using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories;

public class ServiceExpensePaymentRepository : Repository<ServiceExpensePayment>, IServiceExpensePaymentRepository
{
    public ServiceExpensePaymentRepository(CondominioContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ServiceExpensePayment>> GetByServiceExpenseIdAsync(int serviceExpenseId)
    {
        return await _context.ServiceExpensePayments
            .Include(sep => sep.ServiceExpense)
            .Include(sep => sep.Payment)
            .Where(sep => sep.ServiceExpenseId == serviceExpenseId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceExpensePayment>> GetByPaymentIdAsync(int paymentId)
    {
        return await _context.ServiceExpensePayments
            .Include(sep => sep.ServiceExpense)
            .Include(sep => sep.Payment)
            .Where(sep => sep.PaymentId == paymentId)
            .ToListAsync();
    }

    public async Task<ServiceExpensePayment?> GetByServiceExpenseAndPaymentIdAsync(int serviceExpenseId, int paymentId)
    {
        return await _context.ServiceExpensePayments
            .Include(sep => sep.ServiceExpense)
            .Include(sep => sep.Payment)
            .FirstOrDefaultAsync(sep => sep.ServiceExpenseId == serviceExpenseId && sep.PaymentId == paymentId);
    }
}