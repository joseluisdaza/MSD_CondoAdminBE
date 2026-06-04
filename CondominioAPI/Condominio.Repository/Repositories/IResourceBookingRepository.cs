using Condominio.Models;

namespace Condominio.Repository.Repositories
{
  public interface IResourceBookingRepository : IRepository<ResourceBooking>
  {
    /// <summary>
    /// Obtiene las reservas que se solapan con el horario especificado
    /// Excluye reservas canceladas (StatusId = 4)
    /// </summary>
    Task<bool> HasBookingConflictsAsync(
      int resourceId, 
      DateTime startDateTime, 
      DateTime endDateTime, 
      int? excludeBookingId = null);
  }
}
