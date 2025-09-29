using System;
using System.Collections.Generic;

namespace Condominio.Data.MySql.Models;

public partial class PropertyType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int? Rooms { get; set; }

    public int? Bathrooms { get; set; }

    public bool? WaterService { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}
