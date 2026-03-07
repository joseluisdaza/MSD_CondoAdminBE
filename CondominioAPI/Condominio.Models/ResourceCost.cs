namespace Condominio.Models;

public partial class ResourceCost
{
  public int Id { get; set; }

  public int ResourceId { get; set; }

  public decimal BookingPrice { get; set; }

  public decimal BookingWarrantyCost { get; set; }

  public DateTime StartDate { get; set; }

  public DateTime? EndDate { get; set; }

  public virtual Resource Resource { get; set; } = null!;
}
