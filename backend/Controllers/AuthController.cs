// Updated AuthController.cs with BCrypt support
// First, add this package to your backend.csproj:
// <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;
using SpaceLogic.Data.Models.Admin;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BCrypt.Net; // Add this using statement

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AdminDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AdminDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                Console.WriteLine($"Login attempt for email: {request.Email}");
                
                // Find user by email
                var user = await _context.Users
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (user == null)
                {
                    Console.WriteLine("User not found");
                    return Unauthorized(new { success = false, message = "Email ou mot de passe incorrect." });
                }

                Console.WriteLine($"User found: {user.Email}, IsActive: {user.IsActive}");
                
                // Verify password with BCrypt
                if (!VerifyPassword(request.Password, user.Password))
                {
                    Console.WriteLine("Password verification failed");
                    return Unauthorized(new { success = false, message = "Email ou mot de passe incorrect." });
                }

                Console.WriteLine("Password verified successfully");

                // Check if user is active
                if (!user.IsActive)
                {
                    Console.WriteLine("User is not active");
                    return Unauthorized(new { success = false, message = "Compte désactivé. Contactez votre administrateur." });
                }

                // Update last active
                user.LastActive = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(user);

                var response = new LoginResponse
                {
                    Success = true,
                    Message = "Connexion réussie",
                    Token = token,
                    User = new UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Role = user.OrgRole,
                        Department = user.Department,
                        Location = user.Location,
                        OrganizationId = user.OrganizationId,
                        OrganizationName = user.Organization?.Name
                    }
                };

                Console.WriteLine("Login successful, returning response");
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Login error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { success = false, message = "Erreur interne du serveur." });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { success = true, message = "Déconnexion réussie" });
        }

        // Updated password verification method using BCrypt
        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Try BCrypt verification first
                Console.WriteLine($"Verifying password. Hash starts with: {hashedPassword.Substring(0, Math.Min(10, hashedPassword.Length))}");
                
                if (hashedPassword.StartsWith("$2a$") || hashedPassword.StartsWith("$2b$") || hashedPassword.StartsWith("$2y$"))
                {
                    // This is a BCrypt hash
                    bool isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                    Console.WriteLine($"BCrypt verification result: {isValid}");
                    return isValid;
                }
                else
                {
                    // Fallback: check if it's plain text (for development/testing)
                    Console.WriteLine("Hash doesn't look like BCrypt, trying plain text comparison");
                    bool plainTextMatch = password == hashedPassword;
                    Console.WriteLine($"Plain text comparison result: {plainTextMatch}");
                    return plainTextMatch;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password verification error: {ex.Message}");
                return false;
            }
        }

        // Helper method to hash passwords with BCrypt (for future use)
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12); // 12 rounds is secure
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? _configuration["Jwt__Key"] ?? "your-secret-key-here-make-it-long-and-secure-fallback-key-for-development";
            
            if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 16)
            {
                throw new InvalidOperationException("JWT Key must be at least 16 characters long.");
            }
            
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim("OrganizationId", user.OrganizationId.ToString()),
                    new Claim(ClaimTypes.Role, user.OrgRole ?? "User")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Debug endpoint to see user information
        [HttpGet("debug-users")]
        public async Task<IActionResult> DebugUsers()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Organization)
                    .Select(u => new { 
                        u.Email, 
                        u.FirstName, 
                        u.LastName, 
                        u.IsActive,
                        u.OrgRole,
                        OrganizationName = u.Organization != null ? u.Organization.Name : "No Organization",
                        PasswordLength = u.Password.Length,
                        PasswordPrefix = u.Password.Length > 4 ? u.Password.Substring(0, 4) : u.Password,
                        IsBcryptHash = u.Password.StartsWith("$2a$") || u.Password.StartsWith("$2b$") || u.Password.StartsWith("$2y$")
                    })
                    .ToListAsync();
                
                return Ok(new { 
                    message = "Debug info for existing users",
                    userCount = users.Count,
                    users = users 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // Test password verification endpoint
        [HttpPost("test-password")]
        public async Task<IActionResult> TestPassword([FromBody] TestPasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
                
                if (user == null)
                {
                    return NotFound(new { message = "User not found with that email" });
                }

                var results = new
                {
                    userFound = true,
                    isActive = user.IsActive,
                    storedPasswordPrefix = user.Password.Length > 10 ? user.Password.Substring(0, 10) + "..." : user.Password,
                    inputPassword = request.Password,
                    isBcryptHash = user.Password.StartsWith("$2a$") || user.Password.StartsWith("$2b$") || user.Password.StartsWith("$2y$"),
                    bcryptVerification = user.Password.StartsWith("$2") ? BCrypt.Net.BCrypt.Verify(request.Password, user.Password) : false,
                    plainTextMatch = user.Password == request.Password
                };

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    // Request/Response models
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public UserDto? User { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public Guid OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
    }

    public class TestPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}