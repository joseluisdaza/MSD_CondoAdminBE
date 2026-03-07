namespace Condominio.DTOs
{
  public class ResourceCostRequest
  {
    public int Id { get; set; }

    public int ResourceId { get; set; }

    public decimal BookingPrice { get; set; }

    public decimal BookingWarrantyCost { get; set; }

    public DateTime StartDate { get; set; }
  }

  public class FullResourceCostRequest : ResourceCostRequest
  {
    public DateTime? EndDate { get; set; }
  }
}
