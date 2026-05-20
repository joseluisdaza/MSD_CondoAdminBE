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
      {
        return BadRequest(new { message = "StartTime and EndTime must be in HH:mm format." });
      }

      if (startTime >= endTime)
        return BadRequest(new { message = "StartTime must be before EndTime." });

      // Set default StatusId if not provided
      if (resourceBooking.StatusId <= 0)
        resourceBooking.StatusId = 1; // Default to pending status

      // Combine booking date with times for storage
      var bookingDateTime = resourceBooking.BookingDate.Add(startTime);
      resourceBooking.BookingDate = bookingDateTime;

      // Store time information in description if not already set
      if (string.IsNullOrWhiteSpace(resourceBooking.BookingDescription))
      {
        resourceBooking.BookingDescription = $"Time: {resourceBooking.StartTime} - {resourceBooking.EndTime}";
        if (!string.IsNullOrWhiteSpace(resourceBooking.Purpose))
          resourceBooking.BookingDescription += $" | Purpose: {resourceBooking.Purpose}";
        if (resourceBooking.NumberOfPeople > 0)
          resourceBooking.BookingDescription += $" | People: {resourceBooking.NumberOfPeople}";
      }

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
      if (id != resourceBooking.Id)
        return BadRequest();

      var existingResourceBooking = await _resourceBookingRepository.GetByIdAsync(id);
      if (existingResourceBooking == null)
        return NotFound();

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      // Si no es administrador, verificar que la reserva pertenezca al usuario
      if (!isAdmin && existingResourceBooking.UserId != currentUserId)
      {
        return Forbid();
      }

      existingResourceBooking.ResourceId = resourceBooking.ResourceId;
      existingResourceBooking.PropertyId = resourceBooking.PropertyId;
      existingResourceBooking.StatusId = resourceBooking.StatusId;
      existingResourceBooking.BookingDate = resourceBooking.BookingDate;
      existingResourceBooking.BookingPrice = resourceBooking.BookingPrice;
      existingResourceBooking.BookingWarrantyCost = resourceBooking.BookingWarrantyCost;
      existingResourceBooking.BookingDescription = resourceBooking.BookingDescription;
      existingResourceBooking.BookingPhoto = resourceBooking.BookingPhoto;

      await _resourceBookingRepository.UpdateAsync(existingResourceBooking);
      return Ok();
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
  }
}
