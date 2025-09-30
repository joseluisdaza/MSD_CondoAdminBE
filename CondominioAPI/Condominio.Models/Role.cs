namespace Condominio.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RolName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
