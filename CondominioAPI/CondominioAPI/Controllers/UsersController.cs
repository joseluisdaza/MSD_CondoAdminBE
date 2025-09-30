using Condominio.Data.MySql.Models;
using Condominio.Repository.Repositories;
using CondominioAPI.DTOs;
using CondominioAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<UserRequest>>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var userRequests = users.Select(u => u.ToUserRequest()).ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return NotFound();


            return Ok(user.ToUserRequest());
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
