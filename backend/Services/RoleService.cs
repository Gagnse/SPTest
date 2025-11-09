using backend.Services.Common;
using backend.Services.Interfaces;
using backend.Models.DTOs.Role;
using backend.Models.DTOs.Permission;
using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;
using SpaceLogic.Data.Models.Admin;

namespace backend.Services
{
    public class RoleService : IRoleService
    {
        private readonly AdminDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(AdminDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IResult<RoleListDto>> GetOrganizationRolesAsync(Guid organizationId)
        {
            try
            {
                var roles = await _context.OrganizationRoles
                    .Where(r => r.OrganizationId == organizationId)
                    .Select(r => new RoleDto
                    {
                        Id = r.Id,
                        OrganizationId = r.OrganizationId,
                        Name = r.Name,
                        Description = r.Description,
                        CreatedAt = r.CreatedAt,
                        UserCount = r.UserAssignments!.Count(ua => ua.OrganizationId == organizationId)
                    })
                    .OrderBy(r => r.Name)
                    .ToListAsync();

                var result = new RoleListDto
                {
                    Roles = roles,
                    TotalCount = roles.Count
                };

                return Result<RoleListDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for organization {OrganizationId}", organizationId);
                return Result<RoleListDto>.Failure("Error retrieving roles");
            }
        }

        public async Task<IResult<RoleWithPermissionsDto>> GetRoleByIdAsync(Guid roleId, Guid organizationId)
        {
            try
            {
                var role = await _context.OrganizationRoles
                    .Where(r => r.Id == roleId && r.OrganizationId == organizationId)
                    .Include(r => r.RolePermissions!)
                        .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync();

                if (role == null)
                {
                    return Result<RoleWithPermissionsDto>.Failure("Role not found");
                }

                var permissions = role.RolePermissions!
                    .Select(rp => new PermissionDto
                    {
                        Id = rp.Permission!.Id,
                        Name = rp.Permission.Name,
                        Resource = rp.Permission.Resource,
                        Action = rp.Permission.Action,
                        Description = rp.Permission.Description,
                        CreatedAt = rp.Permission.CreatedAt
                    })
                    .ToList();

                var userCount = await _context.UserOrganizationRoles
                    .CountAsync(uor => uor.RoleId == roleId && uor.OrganizationId == organizationId);

                var dto = new RoleWithPermissionsDto
                {
                    Id = role.Id,
                    OrganizationId = role.OrganizationId,
                    Name = role.Name,
                    Description = role.Description,
                    CreatedAt = role.CreatedAt,
                    Permissions = permissions,
                    UserCount = userCount
                };

                return Result<RoleWithPermissionsDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role {RoleId}", roleId);
                return Result<RoleWithPermissionsDto>.Failure("Error retrieving role");
            }
        }

        public async Task<IResult<RoleDto>> CreateRoleAsync(CreateRoleRequest request, Guid organizationId)
        {
            try
            {
                // Check if role name already exists in organization
                var exists = await _context.OrganizationRoles
                    .AnyAsync(r => r.OrganizationId == organizationId && r.Name == request.Name);

                if (exists)
                {
                    return Result<RoleDto>.Failure("Role with this name already exists in the organization");
                }

                var role = new OrganizationRole
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    Name = request.Name,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrganizationRoles.Add(role);

                // Assign initial permissions if provided
                if (request.PermissionIds.Any())
                {
                    // Validate permissions exist
                    var validPermissionIds = await _context.Permissions
                        .Where(p => request.PermissionIds.Contains(p.Id))
                        .Select(p => p.Id)
                        .ToListAsync();

                    var rolePermissions = validPermissionIds.Select(permId => new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permId,
                        GrantedAt = DateTime.UtcNow
                    }).ToList();

                    _context.RolePermissions.AddRange(rolePermissions);
                }

                await _context.SaveChangesAsync();

                var dto = new RoleDto
                {
                    Id = role.Id,
                    OrganizationId = role.OrganizationId,
                    Name = role.Name,
                    Description = role.Description,
                    CreatedAt = role.CreatedAt,
                    UserCount = 0
                };

                _logger.LogInformation("Created role {RoleName} in organization {OrganizationId}", 
                    role.Name, organizationId);

                return Result<RoleDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role {RoleName}", request.Name);
                return Result<RoleDto>.Failure("Error creating role");
            }
        }

        public async Task<IResult<RoleDto>> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, Guid organizationId)
        {
            try
            {
                var role = await _context.OrganizationRoles
                    .FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId);

                if (role == null)
                {
                    return Result<RoleDto>.Failure("Role not found");
                }

                // Update name if provided
                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    // Check if new name conflicts with existing role
                    var nameExists = await _context.OrganizationRoles
                        .AnyAsync(r => r.OrganizationId == organizationId 
                            && r.Name == request.Name 
                            && r.Id != roleId);

                    if (nameExists)
                    {
                        return Result<RoleDto>.Failure("Role with this name already exists in the organization");
                    }

                    role.Name = request.Name;
                }

                // Update description
                if (request.Description != null)
                {
                    role.Description = request.Description;
                }

                await _context.SaveChangesAsync();

                var userCount = await _context.UserOrganizationRoles
                    .CountAsync(uor => uor.RoleId == roleId && uor.OrganizationId == organizationId);

                var dto = new RoleDto
                {
                    Id = role.Id,
                    OrganizationId = role.OrganizationId,
                    Name = role.Name,
                    Description = role.Description,
                    CreatedAt = role.CreatedAt,
                    UserCount = userCount
                };

                _logger.LogInformation("Updated role {RoleId}", roleId);
                return Result<RoleDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {RoleId}", roleId);
                return Result<RoleDto>.Failure("Error updating role");
            }
        }

        public async Task<IResult<bool>> DeleteRoleAsync(Guid roleId, Guid organizationId)
        {
            try
            {
                var role = await _context.OrganizationRoles
                    .FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId);

                if (role == null)
                {
                    return Result<bool>.Failure("Role not found");
                }

                // Check if role is assigned to any users
                var hasUsers = await _context.UserOrganizationRoles
                    .AnyAsync(uor => uor.RoleId == roleId && uor.OrganizationId == organizationId);

                if (hasUsers)
                {
                    return Result<bool>.Failure("Cannot delete role that is assigned to users. Please reassign users first.");
                }

                _context.OrganizationRoles.Remove(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted role {RoleId}", roleId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", roleId);
                return Result<bool>.Failure("Error deleting role");
            }
        }

        public async Task<IResult<List<PermissionDto>>> GetRolePermissionsAsync(Guid roleId)
        {
            try
            {
                var permissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .Include(rp => rp.Permission)
                    .Select(rp => new PermissionDto
                    {
                        Id = rp.Permission!.Id,
                        Name = rp.Permission.Name,
                        Resource = rp.Permission.Resource,
                        Action = rp.Permission.Action,
                        Description = rp.Permission.Description,
                        CreatedAt = rp.Permission.CreatedAt
                    })
                    .OrderBy(p => p.Resource)
                    .ThenBy(p => p.Action)
                    .ToListAsync();

                return Result<List<PermissionDto>>.Success(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions for role {RoleId}", roleId);
                return Result<List<PermissionDto>>.Failure("Error retrieving role permissions");
            }
        }

        public async Task<IResult<bool>> AssignPermissionsToRoleAsync(
            Guid roleId, 
            UpdateRolePermissionsRequest request, 
            Guid organizationId)
        {
            try
            {
                var role = await _context.OrganizationRoles
                    .FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId);

                if (role == null)
                {
                    return Result<bool>.Failure("Role not found");
                }

                // Remove existing permissions
                var existingPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();

                _context.RolePermissions.RemoveRange(existingPermissions);

                // Validate new permissions exist
                var validPermissionIds = await _context.Permissions
                    .Where(p => request.PermissionIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                // Add new permissions
                var newRolePermissions = validPermissionIds.Select(permId => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permId,
                    GrantedAt = DateTime.UtcNow
                }).ToList();

                _context.RolePermissions.AddRange(newRolePermissions);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated permissions for role {RoleId}: {Count} permissions assigned", 
                    roleId, validPermissionIds.Count);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to role {RoleId}", roleId);
                return Result<bool>.Failure("Error assigning permissions to role");
            }
        }

        public async Task<IResult<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid organizationId)
        {
            try
            {
                // Verify user exists and belongs to organization
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.OrganizationId == organizationId);

                if (user == null)
                {
                    return Result<bool>.Failure("User not found in this organization");
                }

                // Verify role exists and belongs to organization
                var role = await _context.OrganizationRoles
                    .FirstOrDefaultAsync(r => r.Id == roleId && r.OrganizationId == organizationId);

                if (role == null)
                {
                    return Result<bool>.Failure("Role not found in this organization");
                }

                // Check if assignment already exists
                var exists = await _context.UserOrganizationRoles
                    .AnyAsync(uor => uor.UserId == userId 
                        && uor.OrganizationId == organizationId 
                        && uor.RoleId == roleId);

                if (exists)
                {
                    return Result<bool>.Failure("User already has this role");
                }

                var userRole = new UserOrganizationRole
                {
                    UserId = userId,
                    OrganizationId = organizationId,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow
                };

                _context.UserOrganizationRoles.Add(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Assigned role {RoleId} to user {UserId}", roleId, userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
                return Result<bool>.Failure("Error assigning role to user");
            }
        }

        public async Task<IResult<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId, Guid organizationId)
        {
            try
            {
                var userRole = await _context.UserOrganizationRoles
                    .FirstOrDefaultAsync(uor => uor.UserId == userId 
                        && uor.OrganizationId == organizationId 
                        && uor.RoleId == roleId);

                if (userRole == null)
                {
                    return Result<bool>.Failure("User does not have this role");
                }

                _context.UserOrganizationRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Removed role {RoleId} from user {UserId}", roleId, userId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
                return Result<bool>.Failure("Error removing role from user");
            }
        }

        public async Task<IResult<List<RoleDto>>> GetUserRolesAsync(Guid userId, Guid organizationId)
        {
            try
            {
                var roles = await _context.UserOrganizationRoles
                    .Where(uor => uor.UserId == userId && uor.OrganizationId == organizationId)
                    .Include(uor => uor.Role)
                    .Select(uor => new RoleDto
                    {
                        Id = uor.Role!.Id,
                        OrganizationId = uor.Role.OrganizationId,
                        Name = uor.Role.Name,
                        Description = uor.Role.Description,
                        CreatedAt = uor.Role.CreatedAt,
                        UserCount = 0 // Not relevant in this context
                    })
                    .ToListAsync();

                return Result<List<RoleDto>>.Success(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
                return Result<List<RoleDto>>.Failure("Error retrieving user roles");
            }
        }
    }
}