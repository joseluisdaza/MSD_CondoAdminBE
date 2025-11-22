using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories;

public class PaymentStatusRepository : Repository<PaymentStatus>, IPaymentStatusRepository
{
    public PaymentStatusRepository(CondominioContext context) : base(context)
    {
    }

    public async Task<PaymentStatus?> GetByIdAsync(int id)
    {
        return await _context.PaymentStatuses
            .Include(ps => ps.Expenses)
            .Include(ps => ps.ServiceExpenses)
            .Include(ps => ps.ServicePayments)
            .FirstOrDefaultAsync(ps => ps.Id == id);
    }

    public async Task<IEnumerable<PaymentStatus>> GetAllAsync()
    {
        return await _context.PaymentStatuses
            .Include(ps => ps.Expenses)
            .Include(ps => ps.ServiceExpenses)
            .Include(ps => ps.ServicePayments)
            .ToListAsync();
    }
}