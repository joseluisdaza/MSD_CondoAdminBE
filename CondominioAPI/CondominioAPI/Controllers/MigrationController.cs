using Condominio.Repository.Repositories;
using Condominio.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public MigrationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Migrates plain text passwords to hashed passwords.
        /// WARNING: This endpoint should be called only once and then removed or protected in production.
        /// </summary>
        /// <returns>Result of the migration process</returns>
        [HttpPost("hash-passwords")]
        [AllowAnonymous] // Change to [Authorize] with admin role in production
        public async Task<IActionResult> HashExistingPasswords()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                int migratedCount = 0;
                int alreadyHashedCount = 0;
                int errorCount = 0;
                var errors = new List<string>();

                foreach (var user in users)
                {
                    try
                    {
                        // Check if password is already hashed (BCrypt hashes start with $2a$, $2b$, or $2y$)
                        if (!string.IsNullOrEmpty(user.Password))
                        {
                            if (user.Password.StartsWith("$2a$") || 
                                user.Password.StartsWith("$2b$") || 
                                user.Password.StartsWith("$2y$"))
                            {
                                alreadyHashedCount++;
                            }
                            else
                            {
                                // Hash the plain text password
                                user.Password = PasswordHasher.HashPassword(user.Password);
                                await _userRepository.UpdateAsync(user);
                                migratedCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        errors.Add($"User ID {user.Id} ({user.Login}): {ex.Message}");
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = $"Migration completed successfully",
                    statistics = new
                    {
                        totalUsers = users.Count(),
                        migratedPasswords = migratedCount,
                        alreadyHashed = alreadyHashedCount,
                        errors = errorCount
                    },
                    details = errors.Any() ? new { errorDetails = errors } : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error during password migration",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Checks the status of passwords in the database without making changes.
        /// Useful to know how many passwords need migration before running the actual migration.
        /// </summary>
        /// <returns>Statistics about password hashing status</returns>
        [HttpGet("check-passwords-status")]
        [AllowAnonymous] // Change to [Authorize] with admin role in production
        public async Task<IActionResult> CheckPasswordsStatus()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                int hashedCount = 0;
                int plainTextCount = 0;
                int emptyCount = 0;

                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.Password))
                    {
                        emptyCount++;
                    }
                    else if (user.Password.StartsWith("$2a$") || 
                             user.Password.StartsWith("$2b$") || 
                             user.Password.StartsWith("$2y$"))
                    {
                        hashedCount++;
                    }
                    else
                    {
                        plainTextCount++;
                    }
                }

                return Ok(new
                {
                    totalUsers = users.Count(),
                    passwordsAlreadyHashed = hashedCount,
                    passwordsInPlainText = plainTextCount,
                    emptyPasswords = emptyCount,
                    needsMigration = plainTextCount > 0,
                    recommendation = plainTextCount > 0 
                        ? "?? Run POST /api/migration/hash-passwords to migrate plain text passwords"
                        : "? All passwords are already hashed. No migration needed."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error checking password status",
                    error = ex.Message
                });
            }
        }
    }
}
