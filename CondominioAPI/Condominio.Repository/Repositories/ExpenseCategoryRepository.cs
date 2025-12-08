using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
    public class ExpenseCategoryRepository : Repository<ExpenseCategory>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(CondominioContext context) : base(context) { }

        public async Task<ExpenseCategory?> GetByCategoryAsync(string category)
        {
            return await _context.ExpenseCategories
                .FirstOrDefaultAsync(ec => ec.Category.ToLower() == category.ToLower());
        }

        public async Task<IEnumerable<ExpenseCategory>> GetAllWithExpensesAsync()
        {
            return await _context.ExpenseCategories
                .Include(ec => ec.Expenses)
                .ToListAsync();
        }
    }
}
