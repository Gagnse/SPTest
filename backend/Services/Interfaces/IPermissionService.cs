using backend.Services.Common;
using backend.Models.DTOs.Permission;

namespace backend.Services.Interfaces
{
    /// <summary>
    /// Service for managing permissions and checking user permissions
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Gets all available permissions in the system
        /// </summary>
        Task<IResult<PermissionListDto>> GetAllPermissionsAsync();

        /// <summary>
        /// Gets a specific permission by ID
        /// </summary>
        Task<IResult<PermissionDto>> GetPermissionByIdAsync(Guid permissionId);

        /// <summary>
        /// Creates a new permission (admin only)
        /// </summary>
        Task<IResult<PermissionDto>> CreatePermissionAsync(CreatePermissionRequest request);

        /// <summary>
        /// Checks if a user has a specific permission in an organization
        /// </summary>
        Task<bool> CheckUserPermissionAsync(Guid userId, Guid organizationId, string resource, string action);

        /// <summary>
        /// Gets all permissions for a specific user in an organization
        /// </summary>
        Task<IResult<List<PermissionDto>>> GetUserPermissionsAsync(Guid userId, Guid organizationId);

        /// <summary>
        /// Seeds default permissions into the database (run on startup)
        /// </summary>
        Task<IResult<bool>> SeedDefaultPermissionsAsync();
    }
}