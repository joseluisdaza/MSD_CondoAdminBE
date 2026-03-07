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
  public class ResourceCostsController : ControllerBase
  {
    private readonly IResourceCostRepository _resourceCostRepository;

    public ResourceCostsController(IResourceCostRepository resourceCostRepository)
    {
      _resourceCostRepository = resourceCostRepository;
    }

    /// <summary>
    /// Obtiene todos los costos de recursos
    /// </summary>
    [HttpGet]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<IEnumerable<ResourceCostRequest>>> GetAll()
    {
      Log.Information("GET > ResourceCosts > GetAll. User: {0}", this.User.Identity?.Name);
      var resourceCosts = await _resourceCostRepository.GetAllAsync();
      return Ok(resourceCosts.Select(x => x.ToFullResourceCostRequest()));
    }

    /// <summary>
    /// Obtiene un costo de recurso por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<ResourceCostRequest>> GetById(int id)
    {
      Log.Information("GET > ResourceCosts > ById > User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var resourceCost = await _resourceCostRepository.GetByIdAsync(id);
      if (resourceCost == null)
        return NotFound();

      return Ok(resourceCost.ToResourceCostRequest());
    }

    /// <summary>
    /// Crea un nuevo costo de recurso (Solo Administradores)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<ResourceCostRequest>> Create(ResourceCostRequest resourceCost)
    {
      Log.Information("POST > ResourceCosts > User: {0}", this.User.Identity?.Name);
      var resourceCostEntity = resourceCost.ToResourceCost();
      await _resourceCostRepository.AddAsync(resourceCostEntity);
      return CreatedAtAction(nameof(GetById), new { id = resourceCostEntity.Id }, resourceCostEntity.ToResourceCostRequest());
    }

    /// <summary>
    /// Actualiza un costo de recurso existente (Solo Administradores)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<IActionResult> Update(int id, ResourceCostRequest resourceCost)
    {
      Log.Information("PUT > ResourceCosts > User: {0}", this.User.Identity?.Name);
      if (id != resourceCost.Id)
        return BadRequest();

      var existingResourceCost = await _resourceCostRepository.GetByIdAsync(id);
      if (existingResourceCost == null)
        return NotFound();

      existingResourceCost.ResourceId = resourceCost.ResourceId;
      existingResourceCost.BookingPrice = resourceCost.BookingPrice;
      existingResourceCost.BookingWarrantyCost = resourceCost.BookingWarrantyCost;

      await _resourceCostRepository.UpdateAsync(existingResourceCost);
      return Ok();
    }

    /// <summary>
    /// Elimina un costo de recurso (Soft Delete) (Solo Administradores)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<IActionResult> Delete(int id)
    {
      Log.Information("DELETE > ResourceCosts > User: {0}", this.User.Identity?.Name);
      var resourceCost = await _resourceCostRepository.GetByIdAsync(id);
      if (resourceCost == null)
        return NotFound();

      resourceCost.EndDate = DateTime.Now;
      await _resourceCostRepository.UpdateAsync(resourceCost);
      return Ok();
    }
  }
}
