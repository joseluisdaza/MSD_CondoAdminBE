using Condominio.Repository.Repositories;
using Condominio.Utils;
using CondominioAPI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly string _jwtSecretKey;

        public AuthController(IUserRepository userRepository, ITokenBlacklistService tokenBlacklistService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenBlacklistService = tokenBlacklistService;
            _jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? throw new InvalidOperationException("JWT_SECRET_KEY not configured");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] dto.LoginRequest request)
        {
            // Log del intento de login
            Log.Information("GET > Login > User: {0} from ip: {1}",
                request.Login,
                HttpContext.Connection.RemoteIpAddress?.ToString());

            // Obtener usuario con sus roles
            var user = await _userRepository.GetByLoginAsync(request.Login);

            // Verificar que el usuario existe y que la contraseña coincide con el hash
            if (user != null && PasswordHasher.VerifyPassword(request.Password, user.Password))
            {
                // Cargar roles del usuario
                var userWithRoles = await _userRepository.GetByIdWithRolesAsync(user.Id);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                // Agregar roles activos al token
                if (userWithRoles?.UserRoles != null)
                {
                    var activeRoles = userWithRoles.UserRoles
                        .Where(ur => ur.EndDate == null)
                        .Select(ur => ur.Role.RolName);

                    foreach (var role in activeRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user = new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()
                    }
                });
            }

            // Log de login fallido
            Log.Warning("GET > Login > User: {request.Login}. Failed login", request.Login);

            return Unauthorized();
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Authorization bearer token is required." });

            var token = authorizationHeader["Bearer ".Length..].Trim();
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Authorization bearer token is required." });

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var expiresAtUtc = jwtToken.ValidTo;

            _tokenBlacklistService.RevokeToken(token, expiresAtUtc);

            if (HttpContext.User.Identity?.Name is { } userName)
                Log.Information("POST > Logout > User: {User}", userName);
            else
                Log.Information("POST > Logout > Anonymous token revoked");

            return Ok(new { message = "Logout successful." });
        }
    }
}
