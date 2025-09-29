using System;
using System.Collections.Generic;

namespace Condominio.Data.MySql.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? LegalId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<PropertyOwner> PropertyOwners { get; set; } = new List<PropertyOwner>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
