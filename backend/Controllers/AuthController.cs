// backend/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using backend.Services.Interfaces;
using backend.Models.DTOs.User;
using backend.Models.DTOs.Auth;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "Email and password are required" });
            }

            var result = await _authService.LoginAsync(request);
            return HandleServiceResult(result);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var userId = GetCurrentUserId();
            
            if (userId == Guid.Empty)
            {
                return Ok(new { success = true, message = "Logged out successfully" });
            }

            var result = await _authService.LogoutAsync(userId);
            return HandleServiceResult(result, "Logged out successfully");
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return HandleServiceResult(result);
        }

        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            
            if (userId == Guid.Empty)
            {
                return Unauthorized(new { success = false, message = "Not authenticated" });
            }

            var result = await _authService.GetCurrentUserAsync(userId);
            return HandleServiceResult(result);
        }
    }
}
