using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories;

public class ServicePaymentRepository : Repository<ServicePayment>, IServicePaymentRepository
{
    public ServicePaymentRepository(CondominioContext context) : base(context)
    {
    }

    public async Task<ServicePayment?> GetByIdAsync(int id)
    {
        return await _context.ServicePayments
            .Include(sp => sp.Status)
            .Include(sp => sp.ServiceExpensePayments)
                .ThenInclude(sep => sep.ServiceExpense)
            .FirstOrDefaultAsync(sp => sp.Id == id);
    }

    public async Task<IEnumerable<ServicePayment>> GetAllAsync()
    {
        return await _context.ServicePayments
            .Include(sp => sp.Status)
            .Include(sp => sp.ServiceExpensePayments)
                .ThenInclude(sep => sep.ServiceExpense)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServicePayment>> GetByStatusIdAsync(int statusId)
    {
        return await _context.ServicePayments
            .Include(sp => sp.Status)
            .Include(sp => sp.ServiceExpensePayments)
                .ThenInclude(sep => sep.ServiceExpense)
            .Where(sp => sp.StatusId == statusId)
            .ToListAsync();
    }

    public async Task<ServicePayment?> GetByReceiveNumberAsync(string receiveNumber)
    {
        return await _context.ServicePayments
            .Include(sp => sp.Status)
            .Include(sp => sp.ServiceExpensePayments)
                .ThenInclude(sep => sep.ServiceExpense)
            .FirstOrDefaultAsync(sp => sp.ReceiveNumber == receiveNumber);
    }

    public async Task<IEnumerable<ServicePayment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.ServicePayments
            .Include(sp => sp.Status)
            .Include(sp => sp.ServiceExpensePayments)
                .ThenInclude(sep => sep.ServiceExpense)
            .Where(sp => sp.PaymentDate >= startDate && sp.PaymentDate <= endDate)
            .ToListAsync();
    }
}