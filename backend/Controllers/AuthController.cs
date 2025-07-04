// backend/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;
using SpaceLogic.Data.Models.Admin;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

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
                    Console.WriteLine($"User not found for email: {request.Email}");
                    return Unauthorized(new { success = false, message = "Email ou mot de passe incorrect." });
                }

                Console.WriteLine($"User found: {user.FirstName} {user.LastName}");

                // Verify password - BCrypt comparison
                if (!VerifyPassword(request.Password, user.Password))
                {
                    Console.WriteLine("Password verification failed");
                    return Unauthorized(new { success = false, message = "Email ou mot de passe incorrect." });
                }

                Console.WriteLine("Password verification successful");

                // Check if user is active
                if (!user.IsActive)
                {
                    Console.WriteLine("User account is inactive");
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

                Console.WriteLine("Login successful");
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

        [HttpPost("verify-token")]
        public async Task<ActionResult<TokenVerificationResponse>> VerifyToken()
        {
            try
            {
                // Get the user ID from the JWT token claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { success = false, message = "Token invalide." });
                }

                // Find the user in database
                var user = await _context.Users
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || !user.IsActive)
                {
                    return Unauthorized(new { success = false, message = "Utilisateur introuvable ou inactif." });
                }

                return Ok(new TokenVerificationResponse
                {
                    Success = true,
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
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token verification error: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Erreur interne du serveur." });
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Your password appears to be BCrypt hashed (starts with $2b$12$)
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password verification error: {ex.Message}");
                return false;
            }
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
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    // DTOs
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public UserDto? User { get; set; }
    }

    public class TokenVerificationResponse
    {
        public bool Success { get; set; }
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
}