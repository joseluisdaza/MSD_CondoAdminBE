using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
    public class PropertyOwnerRepository : Repository<PropertyOwner>, IPropertyOwnerRepository
    {
        public PropertyOwnerRepository(CondominioContext context) : base(context) { }

        public async Task<PropertyOwner?> GetByPropertyAndUserIdAsync(int propertyId, int userId)
        {
            return await _context.PropertyOwners
                .Include(po => po.Property)
                .Include(po => po.User)
                .FirstOrDefaultAsync(po => po.PropertyId == propertyId && po.UserId == userId);
        }

        public async Task<IEnumerable<PropertyOwner>> GetByPropertyIdAsync(int propertyId, bool includeFinalized = false)
        {
            IQueryable<PropertyOwner> query = _context.PropertyOwners
                .Include(po => po.Property)
                .Include(po => po.User)
                .Where(po => po.PropertyId == propertyId);

            if (!includeFinalized)
            {
                query = query.Where(po => po.EndDate == null);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<PropertyOwner>> GetByUserIdAsync(int userId, bool includeFinalized = false)
        {
            IQueryable<PropertyOwner> query = _context.PropertyOwners
                .Include(po => po.Property)
                .Include(po => po.User)
                .Where(po => po.UserId == userId);

            if (!includeFinalized)
            {
                query = query.Where(po => po.EndDate == null);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<PropertyOwner>> GetAllWithRelationsAsync(bool includeFinalized = false)
        {
            IQueryable<PropertyOwner> query = _context.PropertyOwners
                .Include(po => po.Property)
                .Include(po => po.User);

            if (!includeFinalized)
            {
                query = query.Where(po => po.EndDate == null);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<PropertyOwner>> GetFilteredAsync(int? propertyId = null, int? userId = null, bool includeFinalized = false)
        {
            IQueryable<PropertyOwner> query = _context.PropertyOwners
                .Include(po => po.Property)
                .Include(po => po.User);

            if (propertyId.HasValue)
            {
                query = query.Where(po => po.PropertyId == propertyId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(po => po.UserId == userId.Value);
            }

            if (!includeFinalized)
            {
                query = query.Where(po => po.EndDate == null);
            }

            return await query.ToListAsync();
        }
    }
}
