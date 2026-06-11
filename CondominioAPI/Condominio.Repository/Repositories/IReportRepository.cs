using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IReportRepository : IRepository<Report>
  {
    Task<Report?> GetByIdWithRolesAsync(int id);
    Task<IEnumerable<object>> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<Report?> GetByNameAsync(string name);
  }
}
