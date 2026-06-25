using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
  public class StyleRepository : Repository<Style>, IStyleRepository
  {
    public StyleRepository(CondominioContext context) : base(context) { }

    public async Task<Style?> GetByNameAsync(string styleName)
    {
      return await _context.Styles
        .FirstOrDefaultAsync(s => s.StyleName.ToLower() == styleName.ToLower());
    }
  }
}
