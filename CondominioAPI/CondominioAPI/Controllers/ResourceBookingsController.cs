using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace CondominioAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ResourceBookingsController : ControllerBase
  {
    private readonly IResourceBookingRepository _resourceBookingRepository;

    public ResourceBookingsController(IResourceBookingRepository resourceBookingRepository)
    {
      _resourceBookingRepository = resourceBookingRepository;
    }

    /// <summary>
    /// Obtiene todas las reservas de recursos (Solo Administradores)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<IEnumerable<ResourceBookingRequest>>> GetAll()
    {
      Log.Information("GET > ResourceBookings > GetAll. User: {0}", this.User.Identity?.Name);
      var resourceBookings = await _resourceBookingRepository.GetAllAsync();
      return Ok(resourceBookings.Select(x => x.ToResourceBookingRequest()));
    }

    /// <summary>
    /// Verifica la disponibilidad de un recurso en un horario específico
    /// </summary>
    [HttpPost("CheckAvailability")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
    public async Task<ActionResult<ResourceAvailabilityResponse>> CheckAvailability(ResourceAvailabilityRequest request)
    {
      Log.Information("POST > ResourceBookings > CheckAvailability > User: {0}, ResourceId: {1}",
        this.User.Identity?.Name, request.ResourceId);

      // Validate model state
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      DateTime startTime = request.BookingStart;
      DateTime endTime = request.BookingEnd ?? (request.BookingStart.Date.AddDays(1));

      if (startTime < DateTime.Now)
        return BadRequest(new { message = "BookingStart must be in the future." });

      if (endTime <= startTime)
        return BadRequest(new { message = "BookingEnd must be after BookingStart." });

      // Check for conflicting bookings
      bool hasConflicts = await _resourceBookingRepository.HasBookingConflictsAsync(
        request.ResourceId,
        startTime,
        endTime,
        request.ExcludeBookingId);

      return Ok(new ResourceAvailabilityResponse
      {
        IsAvailable = !hasConflicts,
        Message = string.Format("El recurso {0}está disponible en el horario seleccionado.", hasConflicts ? "no " : ""),
      });
    }

    /// <summary>
    /// Obtiene las reservas del usuario actual
    /// </summary>
    [HttpGet("MyBookings")]
    [Authorize(Roles = $"{AppRoles.Habitante},{AppRoles.Administrador}")]
    public async Task<ActionResult<IEnumerable<ResourceBookingRequest>>> GetMyBookings()
    {
      Log.Information("GET > ResourceBookings > MyBookings > User: {0}", this.User.Identity?.Name);
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      if (isAdmin)
      {
        var allBookings = await _resourceBookingRepository.GetAllAsync();
        return Ok(allBookings.Select(x => x.ToResourceBookingRequest()));
      }

      var bookings = await _resourceBookingRepository.GetAllAsync();
      var userBookings = bookings.Where(b => b.UserId == currentUserId);
      return Ok(userBookings.Select(x => x.ToResourceBookingRequest()));
    }

    /// <summary>
    /// Obtiene una reserva de recurso por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
    public async Task<ActionResult<ResourceBookingRequest>> GetById(int id)
    {
      Log.Information("GET > ResourceBookings > ById > User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var resourceBooking = await _resourceBookingRepository.GetByIdAsync(id);
      if (resourceBooking == null)
        return NotFound();

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      // Si no es administrador, verificar que la reserva pertenezca al usuario
      if (!isAdmin && resourceBooking.UserId != currentUserId)
      {
        return Forbid();
      }

      return Ok(resourceBooking.ToResourceBookingRequest());
    }

    /// <summary>
    /// Crea una nueva reserva de recurso
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
    public async Task<ActionResult<ResourceBookingRequest>> Create(ResourceBookingRequest resourceBooking)
    {
      Log.Information("POST > ResourceBookings > User: {0}", this.User.Identity?.Name);

      // Validate model state
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Get current user ID from JWT token
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      // If user is not admin or userId is 0, use current user's ID
      if (!isAdmin || resourceBooking.UserId <= 0)
      {
        if (currentUserId <= 0)
          return BadRequest(new { message = "Unable to determine current user. Authentication failed." });

        resourceBooking.UserId = currentUserId;
      }

      // Validate that booking times are valid
      if (!TimeSpan.TryParse(resourceBooking.StartTime, out var startTime) ||
          !TimeSpan.TryParse(resourceBooking.EndTime, out var endTime))
        return BadRequest(new { message = "StartTime and EndTime must be in HH:mm format." });

      if (startTime >= endTime)
        return BadRequest(new { message = "StartTime must be before EndTime." });

      // Set default StatusId if not provided
      if (resourceBooking.StatusId <= 0)
        resourceBooking.StatusId = 1; // Default to pending status

      // Combine booking date with start time for storage
      var bookingDateTime = resourceBooking.BookingDate.Date.Add(startTime);
      resourceBooking.BookingDate = bookingDateTime;

      // If BookingEndDate is not provided, set it to end of the same day with endTime
      if (!resourceBooking.BookingEndDate.HasValue)
        resourceBooking.BookingEndDate = resourceBooking.BookingDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

      // Verificar disponibilidad del recurso
      bool hasConflicts = await _resourceBookingRepository.HasBookingConflictsAsync(
        resourceBooking.ResourceId,
        bookingDateTime,
        resourceBooking.BookingEndDate.Value);

      if (hasConflicts)
      {
        return BadRequest(new 
        {
          message = GetIsAvailableMessage(hasConflicts)
        });
      }

      // Store time information in description if not already set
      if (string.IsNullOrWhiteSpace(resourceBooking.BookingDescription))
        resourceBooking.BookingDescription = string.Empty;

      var resourceBookingEntity = resourceBooking.ToResourceBooking();
      await _resourceBookingRepository.AddAsync(resourceBookingEntity);

      Log.Information("POST > ResourceBookings > Created successfully for User: {0}, ResourceBooking ID: {1}",
        this.User.Identity?.Name, resourceBookingEntity.Id);

      return CreatedAtAction(nameof(GetById), new { id = resourceBookingEntity.Id }, resourceBookingEntity.ToResourceBookingRequest());
    }

    /// <summary>
    /// Actualiza una reserva de recurso existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
    public async Task<IActionResult> Update(int id, ResourceBookingRequest resourceBooking)
    {
      Log.Information("PUT > ResourceBookings > User: {0}", this.User.Identity?.Name);

      // Validate model state
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      //if (id != resourceBooking.Id)
      //  return BadRequest(new { message = "The booking ID in the URL does not match the ID in the request body." });

      var existingResourceBooking = await _resourceBookingRepository.GetByIdAsync(id);
      if (existingResourceBooking == null)
        return NotFound(new { message = "Resource booking not found." });

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      // Si no es administrador, verificar que la reserva pertenezca al usuario
      if (!isAdmin && existingResourceBooking.UserId != currentUserId)
        return Forbid();

      // Validate that booking times are valid if provided
      if (!string.IsNullOrWhiteSpace(resourceBooking.StartTime) && !string.IsNullOrWhiteSpace(resourceBooking.EndTime))
      {
        if (!TimeSpan.TryParse(resourceBooking.StartTime, out var startTime) ||
            !TimeSpan.TryParse(resourceBooking.EndTime, out var endTime))
          return BadRequest(new { message = "StartTime and EndTime must be in HH:mm format." });

        if (startTime >= endTime)
          return BadRequest(new { message = "StartTime must be before EndTime." });

        // Combine booking date with start time for storage
        var bookingDateTime = resourceBooking.BookingDate.Date.Add(startTime);
        existingResourceBooking.BookingDate = bookingDateTime;

        // Update booking end date
        if (resourceBooking.BookingEndDate.HasValue)
          existingResourceBooking.BookingEndDate = resourceBooking.BookingEndDate.Value.Date.Add(endTime);
        else
          // Default to end of the same day as BookingDate with endTime
          existingResourceBooking.BookingEndDate = resourceBooking.BookingDate.Date.Add(endTime);

        // Verificar disponibilidad del recurso (excluyendo la reserva actual)
        bool hasConflicts = await _resourceBookingRepository.HasBookingConflictsAsync(
          resourceBooking.ResourceId,
          bookingDateTime,
          existingResourceBooking.BookingEndDate.Value,
          id); // Excluir esta reserva de la validación

        if (hasConflicts)
        {
          return BadRequest(new
          {
            message = GetIsAvailableMessage(hasConflicts)
          });
        }
      }
      else
      {
        // If times are not provided, just update the date part
        existingResourceBooking.BookingDate = resourceBooking.BookingDate;

        if (resourceBooking.BookingEndDate.HasValue)
        {
          existingResourceBooking.BookingEndDate = resourceBooking.BookingEndDate.Value;
        }
      }

      // Update basic fields
      existingResourceBooking.ResourceId = resourceBooking.ResourceId;
      existingResourceBooking.PropertyId = resourceBooking.PropertyId;
      existingResourceBooking.StatusId = resourceBooking.StatusId > 0 ? resourceBooking.StatusId : existingResourceBooking.StatusId;
      existingResourceBooking.BookingPrice = resourceBooking.BookingPrice;
      existingResourceBooking.BookingWarrantyCost = resourceBooking.BookingWarrantyCost;
      existingResourceBooking.BookingPhoto = resourceBooking.BookingPhoto;
      existingResourceBooking.BookingDescription = resourceBooking.BookingDescription ?? string.Empty;

      await _resourceBookingRepository.UpdateAsync(existingResourceBooking);

      Log.Information("PUT > ResourceBookings > Updated successfully for User: {0}, ResourceBooking ID: {1}",
        this.User.Identity?.Name, id);

      return Ok(new { message = "Resource booking updated successfully." });
    }

    /// <summary>
    /// Elimina una reserva de recurso (Solo Administradores)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<IActionResult> Delete(int id)
    {
      Log.Information("DELETE > ResourceBookings > User: {0}", this.User.Identity?.Name);
      var resourceBooking = await _resourceBookingRepository.GetByIdAsync(id);
      if (resourceBooking == null)
        return NotFound();

      await _resourceBookingRepository.DeleteAsync(resourceBooking);
      return Ok();
    }

    private static string GetIsAvailableMessage(bool hasConflicts)
    {
      return hasConflicts ? "El recurso no está disponible en el horario seleccionado."
        : "El recurso está disponible en el horario seleccionado.";
    }
  }
}
