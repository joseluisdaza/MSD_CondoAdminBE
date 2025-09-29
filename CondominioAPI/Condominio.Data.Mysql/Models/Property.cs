using System;
using System.Collections.Generic;

namespace Condominio.Data.MySql.Models;

public partial class Property
{
    public int Id { get; set; }

    public string? LegalId { get; set; }

    public string Tower { get; set; } = null!;

    public int Floor { get; set; }

    public string Code { get; set; } = null!;

    public int PropertyType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<PropertyOwner> PropertyOwners { get; set; } = new List<PropertyOwner>();

    public virtual PropertyType PropertyTypeNavigation { get; set; } = null!;
}
