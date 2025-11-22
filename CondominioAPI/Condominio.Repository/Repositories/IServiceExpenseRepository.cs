using Condominio.Models;

namespace Condominio.Repository.Repositories;

public interface IServiceExpenseRepository : IRepository<ServiceExpense>
{
    Task<ServiceExpense?> GetByIdAsync(int id);
    Task<IEnumerable<ServiceExpense>> GetAllAsync();
    Task<IEnumerable<ServiceExpense>> GetByServiceTypeIdAsync(int serviceTypeId);
    Task<IEnumerable<ServiceExpense>> GetByStatusIdAsync(int statusId);
    Task<IEnumerable<ServiceExpense>> GetPendingExpensesAsync();
    Task<IEnumerable<ServiceExpense>> GetOverdueExpensesAsync();
}