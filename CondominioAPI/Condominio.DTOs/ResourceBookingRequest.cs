using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ResourceBookingRequest
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "ResourceId is required")]
    public int ResourceId { get; set; }

    public int UserId { get; set; }

    [Required(ErrorMessage = "PropertyId is required")]
    public int PropertyId { get; set; }

    public int StatusId { get; set; } = 1; // Default to pending status

    [Required(ErrorMessage = "BookingDate is required")]
    public DateTime BookingDate { get; set; }

    [Required(ErrorMessage = "StartTime is required")]
    public string StartTime { get; set; } = null!; // Format: "HH:mm"

    [Required(ErrorMessage = "EndTime is required")]
    public string EndTime { get; set; } = null!; // Format: "HH:mm"

    public decimal BookingPrice { get; set; }

    public decimal BookingWarrantyCost { get; set; }

    public string? BookingDescription { get; set; }

    public string? BookingPhoto { get; set; }

    public string? Purpose { get; set; }

    public int NumberOfPeople { get; set; }
  }
}
