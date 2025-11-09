using backend.Services.Common;
using backend.Services.Interfaces;
using backend.Models.DTOs.User;
using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly AdminDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IPermissionService _permissionService;

        public UserService(
            AdminDbContext context, 
            ILogger<UserService> logger,
            IPermissionService permissionService)
        {
            _context = context;
            _logger = logger;
            _permissionService = permissionService;
        }

        public async Task<IResult<UserListDto>> GetAllUsersAsync(Guid organizationId, UserFilterDto filters)
        {
            try
            {
                // Start with base query
                var query = _context.Users
                    .Where(u => u.OrganizationId == organizationId);

                // Apply soft delete filter (can be overridden by IncludeDeleted flag)
                if (!filters.IncludeDeleted)
                {
                    // Soft delete filter is already applied globally by DbContext
                    // No need to add additional filter
                }
                else
                {
                    // Include deleted users - need to ignore query filter
                    query = _context.Users
                        .IgnoreQueryFilters()
                        .Where(u => u.OrganizationId == organizationId);
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
                {
                    var searchLower = filters.SearchTerm.ToLower();
                    query = query.Where(u =>
                        u.FirstName.ToLower().Contains(searchLower) ||
                        u.LastName.ToLower().Contains(searchLower) ||
                        u.Email.ToLower().Contains(searchLower));
                }

                // Apply role filter
                if (!string.IsNullOrWhiteSpace(filters.Role))
                {
                    query = query.Where(u => u.OrgRole == filters.Role);
                }

                // Apply department filter
                if (!string.IsNullOrWhiteSpace(filters.Department))
                {
                    query = query.Where(u => u.Department == filters.Department);
                }

                // Apply isActive filter
                if (filters.IsActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == filters.IsActive.Value);
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply sorting
                query = filters.SortBy?.ToLower() switch
                {
                    "lastname" => filters.SortDescending
                        ? query.OrderByDescending(u => u.LastName)
                        : query.OrderBy(u => u.LastName),
                    "email" => filters.SortDescending
                        ? query.OrderByDescending(u => u.Email)
                        : query.OrderBy(u => u.Email),
                    "createdat" => filters.SortDescending
                        ? query.OrderByDescending(u => u.CreatedAt)
                        : query.OrderBy(u => u.CreatedAt),
                    "lastactive" => filters.SortDescending
                        ? query.OrderByDescending(u => u.LastActive)
                        : query.OrderBy(u => u.LastActive),
                    _ => filters.SortDescending
                        ? query.OrderByDescending(u => u.FirstName)
                        : query.OrderBy(u => u.FirstName)
                };

                // Apply pagination
                var users = await query
                    .Skip((filters.PageNumber - 1) * filters.PageSize)
                    .Take(filters.PageSize)
                    .Include(u => u.Organization)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Role = u.OrgRole,
                        Department = u.Department,
                        Location = u.Location,
                        Phone = u.Phone,
                        AvatarUrl = u.AvatarUrl,
                        OrganizationId = u.OrganizationId,
                        OrganizationName = u.Organization!.Name,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        LastActive = u.LastActive,
                        UpdatedAt = u.UpdatedAt
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize);

                var result = new UserListDto
                {
                    Users = users,
                    TotalCount = totalCount,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = filters.PageNumber > 1,
                    HasNextPage = filters.PageNumber < totalPages
                };

                return Result<UserListDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users for organization {OrganizationId}", organizationId);
                return Result<UserListDto>.Failure("Error retrieving users");
            }
        }

        public async Task<IResult<UserDto>> GetUserByIdAsync(Guid userId, Guid organizationId)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == userId && u.OrganizationId == organizationId)
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync();

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
                    Phone = user.Phone,
                    AvatarUrl = user.AvatarUrl,
                    OrganizationId = user.OrganizationId,
                    OrganizationName = user.Organization?.Name,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastActive = user.LastActive,
                    UpdatedAt = user.UpdatedAt
                };

                return Result<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                return Result<UserDto>.Failure("Error retrieving user");
            }
        }

        public async Task<IResult<UserDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request, Guid organizationId)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == userId && u.OrganizationId == organizationId)
                    .Include(u => u.Organization)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return Result<UserDto>.Failure("User not found");
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(request.FirstName))
                    user.FirstName = request.FirstName;

                if (!string.IsNullOrWhiteSpace(request.LastName))
                    user.LastName = request.LastName;

                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    // Check if email is already taken by another user
                    var emailExists = await _context.Users
                        .AnyAsync(u => u.Email == request.Email && u.Id != userId && u.OrganizationId == organizationId);

                    if (emailExists)
                    {
                        return Result<UserDto>.Failure("Email is already in use by another user");
                    }

                    user.Email = request.Email;
                }

                if (request.Role != null)
                    user.OrgRole = request.Role;

                if (request.Department != null)
                    user.Department = request.Department;

                if (request.Location != null)
                    user.Location = request.Location;

                if (request.Phone != null)
                    user.Phone = request.Phone;

                if (request.AvatarUrl != null)
                    user.AvatarUrl = request.AvatarUrl;

                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.OrgRole,
                    Department = user.Department,
                    Location = user.Location,
                    Phone = user.Phone,
                    AvatarUrl = user.AvatarUrl,
                    OrganizationId = user.OrganizationId,
                    OrganizationName = user.Organization?.Name,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastActive = user.LastActive,
                    UpdatedAt = user.UpdatedAt
                };

                _logger.LogInformation("Updated user {UserId}", userId);
                return Result<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                return Result<UserDto>.Failure("Error updating user");
            }
        }

        public async Task<IResult<bool>> DeactivateUserAsync(Guid userId, Guid organizationId)
        {
            try
            {
                // Need to ignore query filter to find user even if already soft-deleted
                var user = await _context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Id == userId && u.OrganizationId == organizationId);

                if (user == null)
                {
                    return Result<bool>.Failure("User not found");
                }

                if (user.DeletedAt.HasValue)
                {
                    return Result<bool>.Failure("User is already deactivated");
                }

                // Soft delete
                user.DeletedAt = DateTime.UtcNow;
                user.IsActive = false;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Soft deleted user {UserId}", userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", userId);
                return Result<bool>.Failure("Error deactivating user");
            }
        }

        public async Task<IResult<bool>> ActivateUserAsync(Guid userId, Guid organizationId)
        {
            try
            {
                // Need to ignore query filter to find soft-deleted user
                var user = await _context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Id == userId && u.OrganizationId == organizationId);

                if (user == null)
                {
                    return Result<bool>.Failure("User not found");
                }

                if (!user.DeletedAt.HasValue)
                {
                    return Result<bool>.Failure("User is already active");
                }

                // Reactivate user
                user.DeletedAt = null;
                user.IsActive = true;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Reactivated user {UserId}", userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", userId);
                return Result<bool>.Failure("Error activating user");
            }
        }

        public async Task<IResult<bool>> HardDeleteUserAsync(Guid userId, Guid organizationId)
        {
            try
            {
                // Need to ignore query filter to find user
                var user = await _context.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Id == userId && u.OrganizationId == organizationId);

                if (user == null)
                {
                    return Result<bool>.Failure("User not found");
                }

                // Permanent delete
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogWarning("Permanently deleted user {UserId}", userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently deleting user {UserId}", userId);
                return Result<bool>.Failure("Error deleting user");
            }
        }

        public async Task<IResult<bool>> UpdateLastActiveAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return Result<bool>.Failure("User not found");
                }

                user.LastActive = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last active for user {UserId}", userId);
                return Result<bool>.Failure("Error updating last active timestamp");
            }
        }

        public async Task<IResult<UserPermissionDto>> GetUserPermissionsAsync(Guid userId, Guid organizationId)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == userId && u.OrganizationId == organizationId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return Result<UserPermissionDto>.Failure("User not found");
                }

                // Get user's roles in this organization
                var roles = await _context.UserOrganizationRoles
                    .Where(uor => uor.UserId == userId && uor.OrganizationId == organizationId)
                    .Include(uor => uor.Role)
                    .Select(uor => uor.Role!.Name)
                    .ToListAsync();

                // Get user's effective permissions
                var permissionsResult = await _permissionService.GetUserPermissionsAsync(userId, organizationId);

                if (!permissionsResult.IsSuccess)
                {
                    return Result<UserPermissionDto>.Failure(permissionsResult.ErrorMessage ?? "Error retrieving permissions");
                }

                var permissionNames = permissionsResult.Data!.Select(p => p.Name).ToList();

                var result = new UserPermissionDto
                {
                    UserId = user.Id,
                    UserName = $"{user.FirstName} {user.LastName}",
                    UserEmail = user.Email,
                    Roles = roles,
                    Permissions = permissionNames
                };

                return Result<UserPermissionDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions for user {UserId}", userId);
                return Result<UserPermissionDto>.Failure("Error retrieving user permissions");
            }
        }
    }
}