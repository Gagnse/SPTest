using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models.DTOs.Role;
using backend.Services.Interfaces;

namespace backend.Controllers
{
    /// <summary>
    /// Manages organizational roles and role-permission assignments
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(
            IRoleService roleService,
            ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        /// <summary>
        /// Get all roles for organization
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetOrganizationRoles()
        {
            // TODO: Phase 4 - Add [Permission("roles", "read")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "Getting roles for organization {OrgId}", 
                organizationId);

            var result = await _roleService.GetOrganizationRolesAsync(organizationId);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Get role by ID with permissions
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetRoleById([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("roles", "read")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "Getting role {RoleId} for organization {OrgId}", 
                id, organizationId);

            var result = await _roleService.GetRoleByIdAsync(id, organizationId);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Create new role
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            // TODO: Phase 4 - Add [Permission("roles", "create")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            var createdBy = GetCurrentUserId();
            
            _logger.LogInformation(
                "User {UserId} creating role '{RoleName}' for organization {OrgId}", 
                createdBy, request.Name, organizationId);

            var result = await _roleService.CreateRoleAsync(request, organizationId);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Update existing role
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(
            [FromRoute] Guid id, 
            [FromBody] UpdateRoleRequest request)
        {
            // TODO: Phase 4 - Add [Permission("roles", "update")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            var updatedBy = GetCurrentUserId();
            
            _logger.LogInformation(
                "User {UserId} updating role {RoleId}", 
                updatedBy, id);

            var result = await _roleService.UpdateRoleAsync(id, request, organizationId);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Delete role
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("roles", "delete")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            var deletedBy = GetCurrentUserId();
            
            _logger.LogInformation(
                "User {UserId} deleting role {RoleId}", 
                deletedBy, id);

            var result = await _roleService.DeleteRoleAsync(id, organizationId);
            return HandleServiceResult(result, "Role deleted successfully");
        }

        /// <summary>
        /// Get permissions assigned to role
        /// </summary>
        [HttpGet("{id}/permissions")]
        public async Task<ActionResult> GetRolePermissions([FromRoute] Guid id)
        {
            // TODO: Phase 4 - Add [Permission("roles", "read")] attribute
            
            _logger.LogInformation(
                "Getting permissions for role {RoleId}", 
                id);

            var result = await _roleService.GetRolePermissionsAsync(id);
            return HandleServiceResult(result);
        }

        /// <summary>
        /// Update role permissions
        /// </summary>
        [HttpPut("{id}/permissions")]
        public async Task<ActionResult> UpdateRolePermissions(
            [FromRoute] Guid id, 
            [FromBody] UpdateRolePermissionsRequest request)
        {
            // TODO: Phase 4 - Add [Permission("roles", "update")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            var updatedBy = GetCurrentUserId();
            
            _logger.LogInformation(
                "User {UserId} updating permissions for role {RoleId}", 
                updatedBy, id);

            var result = await _roleService.AssignPermissionsToRoleAsync(id, request, organizationId);
            return HandleServiceResult(result, "Role permissions updated successfully");
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        [HttpPost("/api/users/{userId}/roles")]
        public async Task<ActionResult> AssignRoleToUser(
            [FromRoute] Guid userId, 
            [FromBody] AssignRoleRequest request)
        {
            // TODO: Phase 4 - Add [Permission("users", "update")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            var assignedBy = GetCurrentUserId();
            
            _logger.LogInformation(
                "User {AssignedBy} assigning role {RoleId} to user {UserId}", 
                assignedBy, request.RoleId, userId);

            var result = await _roleService.AssignRoleToUserAsync(userId, request.RoleId, organizationId);
            return HandleServiceResult(result, "Role assigned to user successfully");
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpDelete("/api/users/{userId}/roles/{roleId}")]
        public async Task<ActionResult> RemoveRoleFromUser(
            [FromRoute] Guid userId, 
            [FromRoute] Guid roleId)
        {
            // TODO: Phase 4 - Add [Permission("users", "update")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            var removedBy = GetCurrentUserId();
            
            _logger.LogInformation(
                "User {RemovedBy} removing role {RoleId} from user {UserId}", 
                removedBy, roleId, userId);

            var result = await _roleService.RemoveRoleFromUserAsync(userId, roleId, organizationId);
            return HandleServiceResult(result, "Role removed from user successfully");
        }

        /// <summary>
        /// Get user's roles
        /// </summary>
        [HttpGet("/api/users/{userId}/roles")]
        public async Task<ActionResult> GetUserRoles([FromRoute] Guid userId)
        {
            // TODO: Phase 4 - Add [Permission("users", "read")] attribute
            var organizationId = GetCurrentUserOrganizationId();
            
            _logger.LogInformation(
                "Getting roles for user {UserId}", 
                userId);

            var result = await _roleService.GetUserRolesAsync(userId, organizationId);
            return HandleServiceResult(result);
        }
    }
}