// backend/Services/AuthService.cs
using backend.Services.Common;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SpaceLogic.Data.Admin;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using backend.Models.DTOs.Auth;
using backend.Models.DTOs.User;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly AdminDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AdminDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IResult<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);

                // Find user by email
                var user = await _context.Users
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
                    return Result<LoginResponse>.Failure("Invalid email or password");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                    return Result<LoginResponse>.Failure("Invalid email or password");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login failed: User {UserId} is not active", user.Id);
                    return Result<LoginResponse>.Failure("Account is disabled. Contact your administrator");
                }

                // Update last active
                user.LastActive = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(user);

                var response = new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
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

                _logger.LogInformation("Login successful for user {UserId}", user.Id);
                return Result<LoginResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", request.Email);
                return Result<LoginResponse>.Failure("An error occurred during login");
            }
        }

        public async Task<IResult<UserDto>> GetCurrentUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return Result<UserDto>.Failure("User not found");
                }

                var userDto = new UserDto
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
                };

                return Result<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user {UserId}", userId);
                return Result<UserDto>.Failure("Error retrieving user information");
            }
        }

        public async Task<IResult<bool>> LogoutAsync(Guid userId)
        {
            try
            {
                // In a stateless JWT system, logout is typically handled client-side
                // But we can log the logout event for audit purposes
                _logger.LogInformation("User {UserId} logged out", userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", userId);
                return Result<bool>.Failure("Error during logout");
            }
        }

        public async Task<IResult<UserDto>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (existingUser != null)
                {
                    return Result<UserDto>.Failure("User with this email already exists");
                }

                // Hash password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new SpaceLogic.Data.Models.Admin.User
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = hashedPassword,
                    OrganizationId = request.OrganizationId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastActive = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    OrganizationId = user.OrganizationId
                };

                _logger.LogInformation("User registered successfully: {UserId}", user.Id);
                return Result<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return Result<UserDto>.Failure("Error during registration");
            }
        }

        private string GenerateJwtToken(SpaceLogic.Data.Models.Admin.User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? _configuration["Jwt__Key"];
            var key = Encoding.ASCII.GetBytes(jwtKey!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("OrganizationId", user.OrganizationId.ToString()),
                new Claim("Role", user.OrgRole ?? "User")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}