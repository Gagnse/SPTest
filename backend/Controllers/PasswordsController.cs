using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models.DTOs.Auth;
using backend.Services.Interfaces;

namespace backend.Controllers
{
    /// <summary>
    /// Handles password management operations including change, forgot, and reset
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController : BaseController
    {
        private readonly IPasswordService _passwordService;
        private readonly ILogger<PasswordController> _logger;

        public PasswordController(
            IPasswordService passwordService,
            ILogger<PasswordController> _logger)
        {
            _passwordService = passwordService;
            this._logger = _logger;
        }

        /// <summary>
        /// Change password for authenticated user
        /// </summary>
        [HttpPost("change")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("User {UserId} requesting password change", userId);

            var result = await _passwordService.ChangePasswordAsync(userId, request);
            return HandleServiceResult(result, "Password changed successfully");
        }

        /// <summary>
        /// Initiate forgot password flow
        /// </summary>
        [HttpPost("forgot")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            _logger.LogInformation("Forgot password requested for email: {Email}", request.Email);

            var result = await _passwordService.InitiateForgotPasswordAsync(request);
            
            // Always return success to prevent email enumeration attacks
            // The service handles whether email exists or not internally
            return Ok(new { 
                success = true, 
                message = "If an account exists with this email, password reset instructions have been sent." 
            });
        }

        /// <summary>
        /// Reset password using token from email
        /// </summary>
        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            _logger.LogInformation("Password reset attempted with token");

            var result = await _passwordService.ResetPasswordAsync(request);
            return HandleServiceResult(result, "Password reset successfully. You can now log in with your new password.");
        }

        /// <summary>
        /// Validate password strength
        /// </summary>
        [HttpPost("validate")]
        [AllowAnonymous]
        public async Task<ActionResult> ValidatePassword([FromBody] string password)
        {
            var result = await _passwordService.ValidatePasswordStrengthAsync(password);
            return HandleServiceResult(result);
        }
    }
}