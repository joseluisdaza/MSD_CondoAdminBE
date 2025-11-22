using Condominio.Data.MySql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        Task<Expense?> GetByIdWithRelationsAsync(int id);
        Task<IEnumerable<Expense>> GetAllWithRelationsAsync();
        Task<IEnumerable<Expense>> GetByPropertyIdAsync(int propertyId);
        Task<IEnumerable<Expense>> GetByStatusIdAsync(int statusId);
        Task<IEnumerable<Expense>> GetOverdueExpensesAsync();
    }
}
