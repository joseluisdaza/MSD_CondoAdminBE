using System;
using System.Collections.Generic;

namespace Condominio.Data.MySql.Models;

public partial class PropertyOwner
{
    public int PropertyId { get; set; }

    public int UserId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual Property Property { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
