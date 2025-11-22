using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(CondominioContext context) : base(context) { }

        public async Task<Expense?> GetByIdWithRelationsAsync(int id)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Property)
                .Include(e => e.Status)
                .Include(e => e.ExpensePayments)
                    .ThenInclude(ep => ep.Payment)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Expense>> GetAllWithRelationsAsync()
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Property)
                .Include(e => e.Status)
                .Include(e => e.ExpensePayments)
                    .ThenInclude(ep => ep.Payment)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Property)
                .Include(e => e.Status)
                .Include(e => e.ExpensePayments)
                    .ThenInclude(ep => ep.Payment)
                .Where(e => e.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetByStatusIdAsync(int statusId)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Property)
                .Include(e => e.Status)
                .Include(e => e.ExpensePayments)
                    .ThenInclude(ep => ep.Payment)
                .Where(e => e.StatusId == statusId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetOverdueExpensesAsync()
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Property)
                .Include(e => e.Status)
                .Include(e => e.ExpensePayments)
                    .ThenInclude(ep => ep.Payment)
                .Where(e => e.PaymentLimitDate < DateTime.Now && e.StatusId == 1) // 1: Pending
                .ToListAsync();
        }
    }
}
