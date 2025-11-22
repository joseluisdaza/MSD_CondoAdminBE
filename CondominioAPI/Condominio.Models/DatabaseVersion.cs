namespace Condominio.Models;

public partial class DatabaseVersion
{
    public string VersionNumber { get; set; } = null!;

    public DateTime LastUpdated { get; set; }
}