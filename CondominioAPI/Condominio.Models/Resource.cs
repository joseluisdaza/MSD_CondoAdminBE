namespace Condominio.Models;

public partial class Resource
{
  public int Id { get; set; }

  public string Name { get; set; } = null!;

  public string? Description { get; set; }

  public DateTime StartDate { get; set; }

  public DateTime? EndDate { get; set; }

  public string Photo { get; set; } = null!;

  public virtual ICollection<ResourceCost> ResourceCosts { get; set; } = new List<ResourceCost>();

  public virtual ICollection<ResourceBooking> ResourceBookings { get; set; } = new List<ResourceBooking>();
}
