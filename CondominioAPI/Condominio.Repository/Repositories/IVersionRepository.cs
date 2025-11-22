using Condominio.Models;

namespace Condominio.Repository.Repositories;

public interface IVersionRepository : IRepository<DatabaseVersion>
{
    Task<DatabaseVersion?> GetByVersionNumberAsync(string versionNumber);
    Task<DatabaseVersion?> GetLatestVersionAsync();
    Task<IEnumerable<DatabaseVersion>> GetAllVersionsAsync();
}