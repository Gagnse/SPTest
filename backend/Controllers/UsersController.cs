// backend/Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;
using SpaceLogic.Data.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AdminDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AdminDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET api/users/{userId}/organizations
        [HttpGet("{userId}/organizations")]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetUserOrganizations(Guid userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                
                // Only allow users to get their own organizations or if they are admin
                if (currentUserId != userId && !IsAdminUser())
                {
                    return Forbid();
                }

                var organizations = await _context.OrganizationUsers
                    .Where(ou => ou.UserId == userId)
                    .Include(ou => ou.Organization)
                    .Select(ou => new OrganizationDto
                    {
                        Id = ou.Organization!.Id,
                        Name = ou.Organization.Name,
                        LogoUrl = ou.Organization.LogoUrl,
                        IsActive = ou.Organization.IsActive,
                        JoinedAt = ou.JoinedAt
                    })
                    .Where(o => o.IsActive) // Only return active organizations
                    .OrderBy(o => o.Name)
                    .ToListAsync();

                // If user has no organization relationships through OrganizationUser table,
                // fallback to their primary organization from Users table
                if (!organizations.Any())
                {
                    var user = await _context.Users
                        .Include(u => u.Organization)
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user?.Organization != null)
                    {
                        organizations = new List<OrganizationDto>
                        {
                            new OrganizationDto
                            {
                                Id = user.Organization.Id,
                                Name = user.Organization.Name,
                                LogoUrl = user.Organization.LogoUrl,
                                IsActive = user.Organization.IsActive,
                                JoinedAt = user.CreatedAt
                            }
                        };
                    }
                }

                return Ok(organizations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user organizations: {ex.Message}");
                return StatusCode(500, new { message = "Error retrieving user organizations." });
            }
        }

        // POST api/users/switch-organization
        [HttpPost("switch-organization")]
        public async Task<ActionResult<UserDto>> SwitchOrganization([FromBody] SwitchOrganizationRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Verify user has access to the organization
                var hasAccess = await _context.OrganizationUsers
                    .AnyAsync(ou => ou.UserId == currentUserId && ou.OrganizationId == request.OrganizationId);

                if (!hasAccess)
                {
                    // Check if it's their primary organization
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == currentUserId && u.OrganizationId == request.OrganizationId);
                    
                    if (user == null)
                    {
                        return Forbid("You don't have access to this organization.");
                    }
                }

                // Get user with new organization context
                var userWithOrg = await _context.Users
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync(u => u.Id == currentUserId);

                if (userWithOrg == null)
                {
                    return NotFound("User not found.");
                }

                // Get the new organization
                var newOrganization = await _context.Organizations
                    .FirstOrDefaultAsync(o => o.Id == request.OrganizationId);

                if (newOrganization == null)
                {
                    return NotFound("Organization not found.");
                }

                var userDto = new UserDto
                {
                    Id = userWithOrg.Id,
                    FirstName = userWithOrg.FirstName,
                    LastName = userWithOrg.LastName,
                    Email = userWithOrg.Email,
                    OrganizationId = newOrganization.Id,
                    OrganizationName = newOrganization.Name,
                    Role = userWithOrg.OrgRole,
                    Department = userWithOrg.Department,
                    Location = userWithOrg.Location
                };

                // Set the token separately since it's not part of the original UserDto
                Response.Headers.Add("Authorization", $"Bearer {newToken}");
                
                return Ok(new { 
                    user = userDto, 
                    token = newToken,
                    organizationId = newOrganization.Id,
                    organizationName = newOrganization.Name
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error switching organization: {ex.Message}");
                return StatusCode(500, new { message = "Error switching organization." });
            }
        }

        // GET api/users/current
        [HttpGet("current")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    OrganizationId = user.OrganizationId,
                    OrganizationName = user.Organization?.Name,
                    Role = user.OrgRole,
                    Department = user.Department,
                    Location = user.Location
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving current user: {ex.Message}");
                return StatusCode(500, new { message = "Error retrieving current user." });
            }
        }

        // Helper methods
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private Guid GetCurrentUserOrganizationId()
        {
            var orgIdClaim = User.FindFirst("OrganizationId")?.Value;
            return Guid.TryParse(orgIdClaim, out var orgId) ? orgId : Guid.Empty;
        }

        private bool IsAdminUser()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim == "Admin" || roleClaim == "SuperAdmin";
        }

        private string GenerateJwtToken(User user, Organization organization)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }
            
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim("OrganizationId", organization.Id.ToString()),
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
    }

    // DTOs
    public class OrganizationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public class SwitchOrganizationRequest
    {
        public Guid OrganizationId { get; set; }
    }
}