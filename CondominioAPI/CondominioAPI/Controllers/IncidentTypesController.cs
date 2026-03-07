using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CondominioAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class IncidentTypesController : ControllerBase
  {
    private readonly IIncidentTypeRepository _incidentTypeRepository;

    public IncidentTypesController(IIncidentTypeRepository incidentTypeRepository)
    {
      _incidentTypeRepository = incidentTypeRepository;
    }

    /// <summary>
    /// Obtiene todos los tipos de incidentes
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
    public async Task<ActionResult<IEnumerable<IncidentTypeRequest>>> GetAll()
    {
      Log.Information("GET > IncidentTypes > GetAll. User: {0}", this.User.Identity?.Name);
      var incidentTypes = await _incidentTypeRepository.GetAllAsync();
      return Ok(incidentTypes.Select(x => x.ToIncidentTypeRequest()));
    }

    /// <summary>
    /// Obtiene un tipo de incidente por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
    public async Task<ActionResult<IncidentTypeRequest>> GetById(int id)
    {
      Log.Information("GET > IncidentTypes > ById > User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var incidentType = await _incidentTypeRepository.GetByIdAsync(id);
      if (incidentType == null)
        return NotFound();

      return Ok(incidentType.ToIncidentTypeRequest());
    }

    /// <summary>
    /// Crea un nuevo tipo de incidente (Solo Administradores)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<IncidentTypeRequest>> Create(IncidentTypeRequest incidentType)
    {
      Log.Information("POST > IncidentTypes > User: {0}", this.User.Identity?.Name);
      var incidentTypeEntity = incidentType.ToIncidentType();
      await _incidentTypeRepository.AddAsync(incidentTypeEntity);
      return CreatedAtAction(nameof(GetById), new { id = incidentTypeEntity.Id }, incidentTypeEntity.ToIncidentTypeRequest());
    }

    /// <summary>
    /// Actualiza un tipo de incidente existente (Solo Administradores)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<IActionResult> Update(int id, IncidentTypeRequest incidentType)
    {
      Log.Information("PUT > IncidentTypes > User: {0}", this.User.Identity?.Name);
      if (id != incidentType.Id)
        return BadRequest();

      var existingIncidentType = await _incidentTypeRepository.GetByIdAsync(id);
      if (existingIncidentType == null)
        return NotFound();

      existingIncidentType.Type = incidentType.Type;
      existingIncidentType.Description = incidentType.Description;

      await _incidentTypeRepository.UpdateAsync(existingIncidentType);
      return Ok();
    }

    /// <summary>
    /// Elimina un tipo de incidente (Solo Administradores)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<IActionResult> Delete(int id)
    {
      Log.Information("DELETE > IncidentTypes > User: {0}", this.User.Identity?.Name);
      var incidentType = await _incidentTypeRepository.GetByIdAsync(id);
      if (incidentType == null)
        return NotFound();

      await _incidentTypeRepository.DeleteAsync(incidentType);
      return Ok();
    }
  }
}
