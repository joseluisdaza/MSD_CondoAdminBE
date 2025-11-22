using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories;

public class ServiceExpenseRepository : Repository<ServiceExpense>, IServiceExpenseRepository
{
    public ServiceExpenseRepository(CondominioContext context) : base(context)
    {
    }

    public async Task<ServiceExpense?> GetByIdAsync(int id)
    {
        return await _context.ServiceExpenses
            .Include(se => se.ServiceType)
            .Include(se => se.StatusNavigation)
            .Include(se => se.ServiceExpensePayments)
                .ThenInclude(sep => sep.Payment)
            .FirstOrDefaultAsync(se => se.Id == id);
    }

    public async Task<IEnumerable<ServiceExpense>> GetAllAsync()
    {
        return await _context.ServiceExpenses
            .Include(se => se.ServiceType)
            .Include(se => se.StatusNavigation)
            .Include(se => se.ServiceExpensePayments)
                .ThenInclude(sep => sep.Payment)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceExpense>> GetByServiceTypeIdAsync(int serviceTypeId)
    {
        return await _context.ServiceExpenses
            .Include(se => se.ServiceType)
            .Include(se => se.StatusNavigation)
            .Include(se => se.ServiceExpensePayments)
                .ThenInclude(sep => sep.Payment)
            .Where(se => se.ServiceTypeId == serviceTypeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceExpense>> GetByStatusIdAsync(int statusId)
    {
        return await _context.ServiceExpenses
            .Include(se => se.ServiceType)
            .Include(se => se.StatusNavigation)
            .Include(se => se.ServiceExpensePayments)
                .ThenInclude(sep => sep.Payment)
            .Where(se => se.StatusId == statusId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceExpense>> GetPendingExpensesAsync()
    {
        return await _context.ServiceExpenses
            .Include(se => se.ServiceType)
            .Include(se => se.StatusNavigation)
            .Include(se => se.ServiceExpensePayments)
                .ThenInclude(sep => sep.Payment)
            .Where(se => se.Status == 1) // 1: Pending
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceExpense>> GetOverdueExpensesAsync()
    {
        return await _context.ServiceExpenses
            .Include(se => se.ServiceType)
            .Include(se => se.StatusNavigation)
            .Include(se => se.ServiceExpensePayments)
                .ThenInclude(sep => sep.Payment)
            .Where(se => se.PaymentLimitDate < DateTime.Now && se.Status == 1)
            .ToListAsync();
    }
}