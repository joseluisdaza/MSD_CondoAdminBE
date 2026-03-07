using Condominio.DTOs;
using Condominio.Models;
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
        /// Agrega un nuevo costo para un recurso (Solo Administradores)
        /// Finaliza automáticamente el costo actual (si existe) y crea un nuevo registro
        /// </summary>
        [HttpPost("Add/{resourceId}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Add(int resourceId, UpdateResourceCostRequest request)
        {
            Log.Information("POST > ResourceCosts > Add > User: {0}, ResourceId: {1}", this.User.Identity?.Name, resourceId);

            try
            {
                var currentDateTime = DateTime.Now;

                // Buscar si existe un costo actual activo para finalizar
                var currentResourceCost = await _resourceCostRepository.GetCurrentResourceCostAsync(resourceId);
                
                if (currentResourceCost != null)
                {
                    // Finalizar el costo actual poniendo EndDate
                    currentResourceCost.EndDate = currentDateTime;
                    await _resourceCostRepository.UpdateAsync(currentResourceCost);
                    
                    Log.Information("Previous ResourceCost finalized with ID: {PreviousId} for ResourceId: {ResourceId}", 
                        currentResourceCost.Id, resourceId);
                }

                // Crear el nuevo registro con los nuevos montos
                var newResourceCost = new ResourceCost
                {
                    ResourceId = resourceId,
                    BookingPrice = request.BookingPrice,
                    BookingWarrantyCost = request.BookingWarrantyCost,
                    StartDate = currentDateTime,
                    EndDate = null // Nuevo registro activo
                };

                await _resourceCostRepository.AddAsync(newResourceCost);

                Log.Information("New ResourceCost added successfully. New ID: {NewId}, ResourceId: {ResourceId}, Price: {Price}, Warranty: {Warranty}",
                    newResourceCost.Id, resourceId, request.BookingPrice, request.BookingWarrantyCost);

                // Retornar información del proceso
                return Ok(new
                {
                    message = "Nuevo costo de recurso agregado exitosamente",
                    resourceId = resourceId,
                    previousCostId = currentResourceCost?.Id,
                    newCostId = newResourceCost.Id,
                    newCost = newResourceCost.ToResourceCostRequest(),
                    wasExistingCostFinalized = currentResourceCost != null
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding new ResourceCost for ResourceId: {ResourceId}", resourceId);
                return StatusCode(500, "Error interno del servidor al agregar el costo de recurso");
            }
        }

        /// <summary>
        /// Obtiene todos los costos históricos de un recurso específico por ResourceId
        /// </summary>
        [HttpGet("History/{resourceId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
        public async Task<ActionResult<IEnumerable<FullResourceCostRequest>>> GetResourceCostHistory(int resourceId)
        {
            Log.Information("GET > ResourceCosts > History > User: {0}, ResourceId: {1}", this.User.Identity?.Name, resourceId);

            try
            {
                // Obtener todos los costos del recurso (incluyendo finalizados para historial completo)
                var resourceCosts = await _resourceCostRepository.GetByResourceIdAsync(resourceId, includeFinalized: true);
                
                if (!resourceCosts.Any())
                {
                    Log.Information("No resource costs found for ResourceId: {ResourceId}", resourceId);
                    return Ok(new List<FullResourceCostRequest>()); // Retorna lista vacía
                }

                var resourceCostRequests = resourceCosts.Select(rc => rc.ToFullResourceCostRequest()).ToList();
                
                Log.Information("Found {Count} resource costs for ResourceId: {ResourceId}", resourceCostRequests.Count, resourceId);
                
                return Ok(resourceCostRequests);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving resource cost history for ResourceId: {ResourceId}", resourceId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el costo actual (más reciente y activo) de un recurso específico por ResourceId
        /// </summary>
        [HttpGet("Current/{resourceId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
        public async Task<ActionResult<ResourceCostRequest>> GetCurrentResourceCost(int resourceId)
        {
            Log.Information("GET > ResourceCosts > Current > User: {0}, ResourceId: {1}", this.User.Identity?.Name, resourceId);

            try
            {
                // Obtener el costo actual del recurso (más reciente sin EndDate)
                var currentResourceCost = await _resourceCostRepository.GetCurrentResourceCostAsync(resourceId);

                if (currentResourceCost == null)
                {
                    Log.Information("No current resource cost found for ResourceId: {ResourceId}", resourceId);
                    return NotFound(new { 
                        message = $"No se encontró un costo actual para el recurso con ID {resourceId}",
                        resourceId = resourceId
                    });
                }

                var resourceCostRequest = currentResourceCost.ToResourceCostRequest();

                Log.Information("Current resource cost found for ResourceId: {ResourceId}, CostId: {CostId}", resourceId, currentResourceCost.Id);

                return Ok(resourceCostRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving current resource cost for ResourceId: {ResourceId}", resourceId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
