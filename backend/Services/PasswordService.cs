using backend.Services.Common;
using backend.Services.Interfaces;
using backend.Models.DTOs.Auth;
using backend.Models.DTOs.Email;
using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;
using SpaceLogic.Data.Models.Admin;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly AdminDbContext _context;
        private readonly ILogger<PasswordService> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public PasswordService(
            AdminDbContext context,
            ILogger<PasswordService> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IResult<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return Result<bool>.Failure("User not found");
                }

                // Verify current password
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
                {
                    return Result<bool>.Failure("Current password is incorrect");
                }

                // Validate new password strength
                var strengthResult = await ValidatePasswordStrengthAsync(request.NewPassword);
                if (!strengthResult.IsSuccess)
                {
                    return Result<bool>.Failure("New password does not meet strength requirements");
                }

                // Hash and update password
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} changed their password", userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return Result<bool>.Failure("Error changing password");
            }
        }

        public async Task<IResult<bool>> InitiateForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (user == null)
                {
                    // Don't reveal if email exists - security best practice
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
                    return Result<bool>.Success(true);
                }

                // Generate secure token
                var token = GenerateSecureToken();

                // Create email token
                var emailToken = new EmailToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Email = user.Email,
                    Token = token,
                    TokenType = "password_reset",
                    ExpiresAt = DateTime.UtcNow.AddHours(24), // 24 hour expiry
                    CreatedAt = DateTime.UtcNow
                };

                _context.EmailTokens.Add(emailToken);
                await _context.SaveChangesAsync();

                // Send password reset email
                var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5173";
                var resetUrl = $"{baseUrl}/reset-password?token={token}";

                var emailData = new PasswordResetEmailData
                {
                    RecipientEmail = user.Email,
                    RecipientName = $"{user.FirstName} {user.LastName}",
                    ResetToken = token,
                    ResetUrl = resetUrl,
                    ExpiresAt = emailToken.ExpiresAt
                };

                await _emailService.SendPasswordResetEmailAsync(emailData);

                _logger.LogInformation("Password reset initiated for user {UserId}", user.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating password reset for email {Email}", request.Email);
                return Result<bool>.Failure("Error initiating password reset");
            }
        }

        public async Task<IResult<bool>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                // Validate token
                var emailToken = await _context.EmailTokens
                    .FirstOrDefaultAsync(t => t.Token == request.Token && t.TokenType == "password_reset");

                if (emailToken == null)
                {
                    return Result<bool>.Failure("Invalid or expired reset token");
                }

                if (emailToken.IsExpired)
                {
                    return Result<bool>.Failure("Reset token has expired");
                }

                if (emailToken.IsUsed)
                {
                    return Result<bool>.Failure("Reset token has already been used");
                }

                // Find user by email from token
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == emailToken.Email.ToLower());

                if (user == null)
                {
                    return Result<bool>.Failure("User not found");
                }

                // Validate new password strength
                var strengthResult = await ValidatePasswordStrengthAsync(request.NewPassword);
                if (!strengthResult.IsSuccess)
                {
                    return Result<bool>.Failure("Password does not meet strength requirements");
                }

                // Hash and update password
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                // Mark token as used
                emailToken.UsedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} reset their password", user.Id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return Result<bool>.Failure("Error resetting password");
            }
        }

        public Task<IResult<bool>> ValidatePasswordStrengthAsync(string password)
        {
            try
            {
                var errors = new List<string>();

                // Minimum length
                if (password.Length < 8)
                {
                    errors.Add("Password must be at least 8 characters long");
                }

                // Check for uppercase letter
                if (!Regex.IsMatch(password, @"[A-Z]"))
                {
                    errors.Add("Password must contain at least one uppercase letter");
                }

                // Check for lowercase letter
                if (!Regex.IsMatch(password, @"[a-z]"))
                {
                    errors.Add("Password must contain at least one lowercase letter");
                }

                // Check for digit
                if (!Regex.IsMatch(password, @"\d"))
                {
                    errors.Add("Password must contain at least one digit");
                }

                // Check for special character
                if (!Regex.IsMatch(password, @"[@$!%*?&]"))
                {
                    errors.Add("Password must contain at least one special character (@$!%*?&)");
                }

                if (errors.Any())
                {
                    return Task.FromResult<IResult<bool>>(Result<bool>.Failure(errors));
                }

                return Task.FromResult<IResult<bool>>(Result<bool>.Success(true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating password strength");
                return Task.FromResult<IResult<bool>>(Result<bool>.Failure("Error validating password"));
            }
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}