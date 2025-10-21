using Condominio.Repository.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dto = Condominio.DTOs;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly string _jwtSecretKey;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                ?? throw new InvalidOperationException("JWT_SECRET_KEY not configured");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] dto.LoginRequest request)
        {

            var user = await _userRepository.GetByLoginAsync(request.Login);

            if(user != null && user.Password == request.Password)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Login)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized();
        }
    }
}   
