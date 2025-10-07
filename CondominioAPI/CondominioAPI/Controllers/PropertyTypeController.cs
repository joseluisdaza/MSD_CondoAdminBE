using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyTypeController : ControllerBase
    {
        private readonly IPropertyTypeRepository _propertyTypeRepository;

        public PropertyTypeController(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PropertyTypeRequest>>> GetAll()
        {
            var propertyTypes = await _propertyTypeRepository.GetAllAsync();
            return Ok(propertyTypes.Select(x => x.ToPropertyTypeRequest()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PropertyTypeRequest>> GetById(int id)
        {
            var propertyType = await _propertyTypeRepository.GetByIdAsync(id);
            if (propertyType == null)
                return NotFound();
            return Ok(propertyType.ToPropertyTypeRequest());
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PropertyTypeRequest>> Create(PropertyTypeRequest propertyType)
        {
            await _propertyTypeRepository.AddAsync(propertyType.ToPropertyType());
            return CreatedAtAction(nameof(GetById), new { id = propertyType.Id }, propertyType);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, PropertyTypeRequest propertyType)
        {
            if (id != propertyType.Id)
                return BadRequest();

            await _propertyTypeRepository.UpdateAsync(propertyType.ToPropertyType());
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var propertyType = await _propertyTypeRepository.GetByIdAsync(id);
            if (propertyType == null)
                return NotFound();

            propertyType.EndDate = DateTime.Now;
            await _propertyTypeRepository.UpdateAsync(propertyType);
            return Ok();
        }
    }
}
