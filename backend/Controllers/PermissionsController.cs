using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Services.Interfaces;

namespace backend.Controllers
{
    /// <summary>
    /// Manages system permissions for role-based access control
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PermissionsController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionsController> _logger;

        public PermissionsController(
            IPermissionService permissionService,
            ILogger<PermissionsController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        /// <summary>
        /// Get all available permissions in the system
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetAllPermissions()
        {
            // TODO: Phase 4 - Add [Permission("permissions", "read")] attribute
            _logger.LogInformation("Getting all permissions");

            var result = await _permissionService.GetAllPermissionsAsync();
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Seed default permissions into the database
        /// </summary>
        [HttpPost("seed")]
        public async Task<ActionResult> SeedDefaultPermissions()
        {
            // TODO: Phase 4 - Add [Permission("permissions", "seed")] attribute for superadmin only
            var userId = GetCurrentUserId();
            _logger.LogInformation("User {UserId} initiating permission seeding", userId);

            var result = await _permissionService.SeedDefaultPermissionsAsync();
            return HandleServiceResult(result, "Default permissions seeded successfully");
        }
    }
}