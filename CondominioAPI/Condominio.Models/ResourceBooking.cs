namespace Condominio.Models;

public partial class ResourceBooking
{
  public int Id { get; set; }

  public int ResourceId { get; set; }

  public int UserId { get; set; }

  public int PropertyId { get; set; }

  public int StatusId { get; set; }

  public DateTime BookingDate { get; set; }

  public decimal BookingPrice { get; set; }

  public decimal BookingWarrantyCost { get; set; }

  public string? BookingDescription { get; set; }

  public string? BookingPhoto { get; set; }

  public virtual Resource Resource { get; set; } = null!;

  public virtual User User { get; set; } = null!;

  public virtual Property Property { get; set; } = null!;

  public virtual PaymentStatus Status { get; set; } = null!;
}
