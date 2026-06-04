using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Condominio.Repository.Repositories
{
  public class ResourceBookingRepository : Repository<ResourceBooking>, IResourceBookingRepository
  {
    public ResourceBookingRepository(CondominioContext context) : base(context) { }

    /// <summary>
    /// Retorna true si hay reservas que se solapan con el horario especificado para el recurso dado.
    /// Excluye reservas canceladas (StatusId = 4 según PaymentStatus.Cancelled)
    /// </summary>
    public async Task<bool> HasBookingConflictsAsync(int resourceId, DateTime startDateTime, DateTime endDateTime, int? excludeBookingId = null)
    {
      var conflictResourceBookingIds = _context.ResourceBookings
        .Where(rb => rb.ResourceId == resourceId
          && rb.StatusId != 4 // Excluir reservas canceladas
          && (
            // Caso 1: La nueva reserva empieza durante una reserva existente
            (startDateTime >= rb.BookingDate && startDateTime < rb.BookingEndDate)
            ||
            // Caso 2: La nueva reserva termina durante una reserva existente
            (endDateTime > rb.BookingDate && endDateTime <= rb.BookingEndDate)
            ||
            // Caso 3: La nueva reserva envuelve completamente una reserva existente
            (startDateTime <= rb.BookingDate && endDateTime >= rb.BookingEndDate)
          )).Select(rb => rb.ResourceId);

      if (excludeBookingId.HasValue)
        return conflictResourceBookingIds.Any(x => x != excludeBookingId.Value);

      return conflictResourceBookingIds.Any();
    }
  }
}
