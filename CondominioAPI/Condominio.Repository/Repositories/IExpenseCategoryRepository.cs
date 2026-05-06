using Condominio.Data.Mysql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
    public interface IExpenseCategoryRepository : IRepository<ExpenseCategory>
    {
        Task<ExpenseCategory?> GetByCategoryAsync(string category);
        Task<IEnumerable<ExpenseCategory>> GetAllWithExpensesAsync();
    }
}
