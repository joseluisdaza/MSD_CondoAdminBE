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

      // Si no es administrador, el UserId debe ser el del usuario actual
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      if (!isAdmin)
      {
        resourceBooking.UserId = currentUserId;
      }

      var resourceBookingEntity = resourceBooking.ToResourceBooking();
      await _resourceBookingRepository.AddAsync(resourceBookingEntity);
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
