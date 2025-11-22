using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories;

public class VersionRepository : Repository<DatabaseVersion>, IVersionRepository
{
    public VersionRepository(CondominioContext context) : base(context)
    {
    }

    public async Task<DatabaseVersion?> GetByVersionNumberAsync(string versionNumber)
    {
        return await _context.Versions
            .FirstOrDefaultAsync(v => v.VersionNumber == versionNumber);
    }

    public async Task<DatabaseVersion?> GetLatestVersionAsync()
    {
        return await _context.Versions
            .OrderByDescending(v => v.LastUpdated)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<DatabaseVersion>> GetAllVersionsAsync()
    {
        return await _context.Versions
            .OrderByDescending(v => v.LastUpdated)
            .ToListAsync();
    }
}