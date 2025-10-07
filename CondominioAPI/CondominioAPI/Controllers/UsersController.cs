using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserBaseRequest>>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users.Select(u => u.ToUserBaseRequest()).ToList());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserBaseRequest>> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user.ToUserBaseRequest());
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserRequest>> Create(UserRequest user)
        {
            user.EndDate = null;
            var userEntity = user.ToUser();
            await _userRepository.AddAsync(userEntity);
            
            var createdUserRequest = userEntity.ToUserRequest(includeId: true);
            return CreatedAtAction(nameof(GetById), new { id = userEntity.Id }, createdUserRequest);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UserRequest user)
        {
            var userFound = await _userRepository.GetByIdAsync(id);
            if (userFound == null)
                return NotFound();

            user.EndDate = userFound.EndDate;
            await _userRepository.UpdateAsync(user.ToUser());
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.EndDate = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }
    }
}
