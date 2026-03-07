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
  public class IncidentCostsController : ControllerBase
  {
    private readonly IIncidentCostRepository _incidentCostRepository;

    public IncidentCostsController(IIncidentCostRepository incidentCostRepository)
    {
      _incidentCostRepository = incidentCostRepository;
    }

    /// <summary>
    /// Obtiene todos los costos de incidentes
    /// </summary>
    [HttpGet]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<IEnumerable<IncidentCostRequest>>> GetAll()
    {
      Log.Information("GET > IncidentCosts > GetAll. User: {0}", this.User.Identity?.Name);
      var incidentCosts = await _incidentCostRepository.GetAllAsync();
      return Ok(incidentCosts.Select(x => x.ToFullIncidentCostRequest()));
    }

    /// <summary>
    /// Obtiene un costo de incidente por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<IncidentCostRequest>> GetById(int id)
    {
      Log.Information("GET > IncidentCosts > ById > User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var incidentCost = await _incidentCostRepository.GetByIdAsync(id);
      if (incidentCost == null)
        return NotFound();

      return Ok(incidentCost.ToIncidentCostRequest());
    }

    /// <summary>
    /// Crea un nuevo costo de incidente (Solo Administradores)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<IncidentCostRequest>> Create(IncidentCostRequest incidentCost)
    {
      Log.Information("POST > IncidentCosts > User: {0}", this.User.Identity?.Name);
      var incidentCostEntity = incidentCost.ToIncidentCost();
      await _incidentCostRepository.AddAsync(incidentCostEntity);
      return CreatedAtAction(nameof(GetById), new { id = incidentCostEntity.Id }, incidentCostEntity.ToIncidentCostRequest());
    }

    /// <summary>
    /// Actualiza un costo de incidente existente (Solo Administradores)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<IActionResult> Update(int id, IncidentCostRequest incidentCost)
    {
      Log.Information("PUT > IncidentCosts > User: {0}", this.User.Identity?.Name);
      if (id != incidentCost.Id)
        return BadRequest();

      var existingIncidentCost = await _incidentCostRepository.GetByIdAsync(id);
      if (existingIncidentCost == null)
        return NotFound();

      existingIncidentCost.IncidentTypeId = incidentCost.IncidentTypeId;
      existingIncidentCost.Cost = incidentCost.Cost;
      existingIncidentCost.Description = incidentCost.Description;

      await _incidentCostRepository.UpdateAsync(existingIncidentCost);
      return Ok();
    }

    /// <summary>
    /// Elimina un costo de incidente (Soft Delete) (Solo Administradores)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<IActionResult> Delete(int id)
    {
      Log.Information("DELETE > IncidentCosts > User: {0}", this.User.Identity?.Name);
      var incidentCost = await _incidentCostRepository.GetByIdAsync(id);
      if (incidentCost == null)
        return NotFound();

      incidentCost.EndDate = DateTime.Now;
      await _incidentCostRepository.UpdateAsync(incidentCost);
      return Ok();
    }
  }
}
