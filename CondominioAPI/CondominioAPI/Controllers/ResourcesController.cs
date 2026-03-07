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
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceRepository _resourceRepository;

        public ResourcesController(IResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        /// <summary>
        /// Obtiene todos los recursos
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
        public async Task<ActionResult<IEnumerable<ResourceRequest>>> GetAll()
        {
            Log.Information("GET > Resources > GetAll. User: {0}", this.User.Identity?.Name);
            var resources = await _resourceRepository.GetAllAsync();
            return Ok(resources.Select(x => x.ToFullResourceRequest()));
        }

        /// <summary>
        /// Obtiene un recurso por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]
        public async Task<ActionResult<ResourceRequest>> GetById(int id)
        {
            Log.Information("GET > Resources > ById > User: {0}, Id: {1}", this.User.Identity?.Name, id);

            var resource = await _resourceRepository.GetByIdAsync(id);
            if (resource == null)
                return NotFound();

            return Ok(resource.ToResourceRequest());
        }

        /// <summary>
        /// Crea un nuevo recurso (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<ActionResult<ResourceRequest>> Create(ResourceRequest resource)
        {
            Log.Information("POST > Resources > User: {0}", this.User.Identity?.Name);
            var resourceEntity = resource.ToResource();
            await _resourceRepository.AddAsync(resourceEntity);
            return CreatedAtAction(nameof(GetById), new { id = resourceEntity.Id }, resourceEntity.ToResourceRequest());
        }

        /// <summary>
        /// Actualiza un recurso existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Update(int id, ResourceRequest resource)
        {
            Log.Information("PUT > Resources > User: {0}", this.User.Identity?.Name);
            if (id != resource.Id)
                return BadRequest();

            var existingResource = await _resourceRepository.GetByIdAsync(id);
            if (existingResource == null)
                return NotFound();

            existingResource.Name = resource.Name;
            existingResource.Description = resource.Description;
            existingResource.Photo = resource.Photo;

            await _resourceRepository.UpdateAsync(existingResource);
            return Ok();
        }

        /// <summary>
        /// Elimina un recurso (Soft Delete) (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Administrador)]
        public async Task<IActionResult> Delete(int id)
        {
            Log.Information("DELETE > Resources > User: {0}", this.User.Identity?.Name);
            var resource = await _resourceRepository.GetByIdAsync(id);
            if (resource == null)
                return NotFound();

            resource.EndDate = DateTime.Now;
            await _resourceRepository.UpdateAsync(resource);
            return Ok();
        }
    }
}   
