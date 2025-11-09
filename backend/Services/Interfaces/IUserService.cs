using backend.Services.Common;
using backend.Models.DTOs.User;

namespace backend.Services.Interfaces
{
    /// <summary>
    /// Service for managing users within an organization
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets all users in an organization with filtering, pagination, and sorting
        /// </summary>
        Task<IResult<UserListDto>> GetAllUsersAsync(Guid organizationId, UserFilterDto filters);

        /// <summary>
        /// Gets a specific user by ID
        /// </summary>
        Task<IResult<UserDto>> GetUserByIdAsync(Guid userId, Guid organizationId);

        /// <summary>
        /// Updates a user's profile information
        /// </summary>
        Task<IResult<UserDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request, Guid organizationId);

        /// <summary>
        /// Soft deletes a user (sets deleted_at timestamp)
        /// </summary>
        Task<IResult<bool>> DeactivateUserAsync(Guid userId, Guid organizationId);

        /// <summary>
        /// Reactivates a soft-deleted user (clears deleted_at)
        /// </summary>
        Task<IResult<bool>> ActivateUserAsync(Guid userId, Guid organizationId);

        /// <summary>
        /// Permanently deletes a user from the database (admin only)
        /// </summary>
        Task<IResult<bool>> HardDeleteUserAsync(Guid userId, Guid organizationId);

        /// <summary>
        /// Updates the user's last active timestamp
        /// </summary>
        Task<IResult<bool>> UpdateLastActiveAsync(Guid userId);

        /// <summary>
        /// Gets user's effective permissions in organization
        /// </summary>
        Task<IResult<UserPermissionDto>> GetUserPermissionsAsync(Guid userId, Guid organizationId);
    }
}