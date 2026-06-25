using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IStyleRepository : IRepository<Style>
  {
    Task<Style?> GetByNameAsync(string styleName);
  }
}
