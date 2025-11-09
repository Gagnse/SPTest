using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models.DTOs.User;
using backend.Services.Interfaces;

namespace backend.Controllers
{
    /// <summary>
    /// Manages user accounts and profiles
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get all users in organization with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetAllUsers([FromQuery] UserFilterDto filters)
        {
            // TODO: Phase 4 - Add [Permission("users", "read")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "Getting users for organization {OrgId} with filters", 
                organizationId);

            var result = await _userService.GetAllUsersAsync(organizationId, filters);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("users", "read")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "Getting user {UserId} for organization {OrgId}", 
                id, organizationId);

            var result = await _userService.GetUserByIdAsync(id, organizationId);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(
            [FromRoute] Guid id, 
            [FromBody] UpdateUserRequest request)
        {
            // TODO: Phase 4 - Add [Permission("users", "update")] attribute
            // Users can update their own profile, admins can update any user
            var currentUserId = GetCurrentUserId();
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "User {CurrentUserId} updating user {UserId}", 
                currentUserId, id);

            var result = await _userService.UpdateUserAsync(id, request, organizationId);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Soft delete user (deactivate)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeactivateUser([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("users", "delete")] attribute
            var currentUserId = GetCurrentUserId();
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "User {CurrentUserId} deactivating user {UserId}", 
                currentUserId, id);

            var result = await _userService.DeactivateUserAsync(id, organizationId);
            return HandleServiceResult(result, "User deactivated successfully");
        }

        /// <summary>
        /// Permanently delete user (admin only)
        /// </summary>
        [HttpDelete("{id}/permanent")]
        public async Task<ActionResult> HardDeleteUser([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("users", "hard_delete")] attribute (superadmin only)
            var currentUserId = GetCurrentUserId();
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogWarning(
                "User {CurrentUserId} permanently deleting user {UserId}", 
                currentUserId, id);

            var result = await _userService.HardDeleteUserAsync(id, organizationId);
            return HandleServiceResult(result, "User permanently deleted");
        }

        /// <summary>
        /// Reactivate soft-deleted user
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<ActionResult> ActivateUser([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("users", "activate")] attribute
            var currentUserId = GetCurrentUserId();
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "User {CurrentUserId} reactivating user {UserId}", 
                currentUserId, id);

            var result = await _userService.ActivateUserAsync(id, organizationId);
            return HandleServiceResult(result, "User reactivated successfully");
        }

        /// <summary>
        /// Get user's effective permissions
        /// </summary>
        [HttpGet("{id}/permissions")]
        public async Task<ActionResult> GetUserPermissions([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("users", "read")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "Getting permissions for user {UserId}", 
                id);

            var result = await _userService.GetUserPermissionsAsync(id, organizationId);
            return HandleServiceResult(result);
        }
    }
}