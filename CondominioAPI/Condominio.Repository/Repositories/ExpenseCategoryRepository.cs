using Condominio.Data.MySql.Models;

namespace Condominio.Repository.Repositories
{
    public class ExpenseCategoryRepository : Repository<ExpenseCategory>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(CondominioContext context) : base(context) { }
        // Add entity-specific methods if needed
    }
}
