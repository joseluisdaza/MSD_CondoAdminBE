using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
    public class PropertyTypeRepository : Repository<PropertyType>, IPropertyTypeRepository
    {
        public PropertyTypeRepository(CondominioContext context) : base(context) { }

        public async Task<PropertyType?> GetByTypeAsync(string type)
        {
            return await _context.PropertyTypes
                .FirstOrDefaultAsync(pt => pt.Type.ToLower() == type.ToLower() && pt.EndDate == null);
        }
    }
}
