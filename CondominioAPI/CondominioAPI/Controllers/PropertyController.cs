using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyRepository _PropertyRepository;

        public PropertyController(IPropertyRepository PropertyRepository)
        {
            _PropertyRepository = PropertyRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PropertyRequest>>> GetAll()
        {
            var Propertys = await _PropertyRepository.GetAllAsync();
            return Ok(Propertys.Select(x => x.ToPropertyRequest()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PropertyRequest>> GetById(int id)
        {
            var Property = await _PropertyRepository.GetByIdAsync(id);
            if (Property == null)
                return NotFound();
            return Ok(Property.ToPropertyRequest());
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PropertyRequest>> Create(PropertyRequest Property)
        {
            await _PropertyRepository.AddAsync(Property.ToProperty());
            return CreatedAtAction(nameof(GetById), new { id = Property.Id }, Property);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, PropertyRequest Property)
        {
            if (id != Property.Id)
                return BadRequest();

            await _PropertyRepository.UpdateAsync(Property.ToProperty());
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var Property = await _PropertyRepository.GetByIdAsync(id);
            if (Property == null)
                return NotFound();

            Property.EndDate = DateTime.Now;
            await _PropertyRepository.UpdateAsync(Property);
            return Ok();
        }
    }
}
