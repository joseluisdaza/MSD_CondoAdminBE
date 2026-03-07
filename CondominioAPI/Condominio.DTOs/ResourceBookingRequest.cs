namespace Condominio.DTOs
{
  public class ResourceBookingRequest
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
  }
}
