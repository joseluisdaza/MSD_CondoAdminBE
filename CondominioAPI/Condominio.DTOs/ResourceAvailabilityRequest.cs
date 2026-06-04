using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ResourceAvailabilityRequest
  {
    [Required(ErrorMessage = "ResourceId is required")]
    public int ResourceId { get; set; }

    [Required(ErrorMessage = "BookingDate is required")]
    public DateTime BookingStart { get; set; }

    public DateTime? BookingEnd { get; set; } // If null, it will default to the next day at 00:00:00

    public int? ExcludeBookingId { get; set; } // Para excluir una reserva al actualizar
  }
}
