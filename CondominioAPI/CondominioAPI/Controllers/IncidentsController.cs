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
  public class IncidentsController : ControllerBase
  {
    private readonly IIncidentRepository _incidentRepository;

    public IncidentsController(IIncidentRepository incidentRepository)
    {
      _incidentRepository = incidentRepository;
    }

    /// <summary>
    /// Obtiene todos los incidentes (Solo Administradores)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<IEnumerable<IncidentRequest>>> GetAll()
    {
      Log.Information("GET > Incidents > GetAll. User: {0}", this.User.Identity?.Name);
      var incidents = await _incidentRepository.GetAllAsync();
      return Ok(incidents.Select(x => x.ToIncidentRequest()));
    }

    /// <summary>
    /// Obtiene los incidentes del usuario actual
    /// </summary>
    [HttpGet("MyIncidents")]
    [Authorize(Roles = $"{AppRoles.Habitante},{AppRoles.Administrador}")]
    public async Task<ActionResult<IEnumerable<IncidentRequest>>> GetMyIncidents()
    {
      Log.Information("GET > Incidents > MyIncidents > User: {0}", this.User.Identity?.Name);
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      if (isAdmin)
      {
        var allIncidents = await _incidentRepository.GetAllAsync();
        return Ok(allIncidents.Select(x => x.ToIncidentRequest()));
      }

      var incidents = await _incidentRepository.GetAllAsync();
      var userIncidents = incidents.Where(i => i.UserId == currentUserId);
      return Ok(userIncidents.Select(x => x.ToIncidentRequest()));
    }

    /// <summary>
    /// Obtiene un incidente por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
    public async Task<ActionResult<IncidentRequest>> GetById(int id)
    {
      Log.Information("GET > Incidents > ById > User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var incident = await _incidentRepository.GetByIdAsync(id);
      if (incident == null)
        return NotFound();

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      // Si no es administrador, verificar que el incidente pertenezca al usuario
      if (!isAdmin && incident.UserId != currentUserId)
      {
        return Forbid();
      }

      return Ok(incident.ToIncidentRequest());
    }

    /// <summary>
    /// Crea un nuevo incidente
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
    public async Task<ActionResult<IncidentRequest>> Create(IncidentRequest incident)
    {
      Log.Information("POST > Incidents > User: {0}", this.User.Identity?.Name);

      // Si no es administrador, el UserId debe ser el del usuario actual
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      if (!isAdmin)
      {
        incident.UserId = currentUserId;
      }

      var incidentEntity = incident.ToIncident();
      await _incidentRepository.AddAsync(incidentEntity);
      return CreatedAtAction(nameof(GetById), new { id = incidentEntity.Id }, incidentEntity.ToIncidentRequest());
    }

    /// <summary>
    /// Actualiza un incidente existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
    public async Task<IActionResult> Update(int id, IncidentRequest incident)
    {
      Log.Information("PUT > Incidents > User: {0}", this.User.Identity?.Name);
      if (id != incident.Id)
        return BadRequest();

      var existingIncident = await _incidentRepository.GetByIdAsync(id);
      if (existingIncident == null)
        return NotFound();

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
      var isAdmin = User.IsInRole(AppRoles.Administrador);

      // Si no es administrador, verificar que el incidente pertenezca al usuario
      if (!isAdmin && existingIncident.UserId != currentUserId)
      {
        return Forbid();
      }

      existingIncident.IncidentTypeId = incident.IncidentTypeId;
      existingIncident.PropertyId = incident.PropertyId;
      existingIncident.StatusId = incident.StatusId;
      existingIncident.IncidentDate = incident.IncidentDate;
      existingIncident.IncidentDescription = incident.IncidentDescription;
      existingIncident.IncidentPhoto = incident.IncidentPhoto;

      await _incidentRepository.UpdateAsync(existingIncident);
      return Ok();
    }

    /// <summary>
    /// Elimina un incidente (Solo Administradores)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<IActionResult> Delete(int id)
    {
      Log.Information("DELETE > Incidents > User: {0}", this.User.Identity?.Name);
      var incident = await _incidentRepository.GetByIdAsync(id);
      if (incident == null)
        return NotFound();

      await _incidentRepository.DeleteAsync(incident);
      return Ok();
    }
  }
}
