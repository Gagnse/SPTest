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
                // Find user by email
                var user = await _context.Users
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (user == null)
                {
                    return Unauthorized(new { success = false, message = "Email ou mot de passe incorrect." });
                }

                // Verify password (assuming passwords are hashed)
                if (!VerifyPassword(request.Password, user.Password))
                {
                    return Unauthorized(new { success = false, message = "Email ou mot de passe incorrect." });
                }

                // Check if user is active
                if (!user.IsActive)
                {
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
                        Role = user.Role,
                        Department = user.Department,
                        Location = user.Location,
                        OrganizationId = user.OrganizationId,
                        OrganizationName = user.Organization?.Name
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception details (consider using ILogger)
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
            // In a real application, you might want to blacklist the token
            // For now, we'll just return a success response
            return Ok(new { success = true, message = "Déconnexion réussie" });
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // For development purposes, we'll do a simple comparison
            // In production, use proper password hashing like BCrypt
            // This is a placeholder - replace with actual password verification
            
            // Simple hash for demo (replace with proper hashing)
            var hashedInput = ComputeSha256Hash(password);
            return hashedInput == hashedPassword || password == hashedPassword;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
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
                    new Claim(ClaimTypes.Role, user.Role ?? "User")
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