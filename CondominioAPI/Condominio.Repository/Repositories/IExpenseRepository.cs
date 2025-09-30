using Condominio.Data.MySql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        // Add entity-specific methods if needed
    }
}
