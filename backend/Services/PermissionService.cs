using backend.Services.Common;
using backend.Services.Interfaces;
using backend.Models.DTOs.Permission;
using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;
using SpaceLogic.Data.Models.Admin;

namespace backend.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AdminDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(AdminDbContext context, ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IResult<PermissionListDto>> GetAllPermissionsAsync()
        {
            try
            {
                var permissions = await _context.Permissions
                    .OrderBy(p => p.Resource)
                    .ThenBy(p => p.Action)
                    .Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Resource = p.Resource,
                        Action = p.Action,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                var result = new PermissionListDto
                {
                    Permissions = permissions,
                    TotalCount = permissions.Count
                };

                return Result<PermissionListDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all permissions");
                return Result<PermissionListDto>.Failure("Error retrieving permissions");
            }
        }

        public async Task<IResult<PermissionDto>> GetPermissionByIdAsync(Guid permissionId)
        {
            try
            {
                var permission = await _context.Permissions
                    .Where(p => p.Id == permissionId)
                    .Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Resource = p.Resource,
                        Action = p.Action,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (permission == null)
                {
                    return Result<PermissionDto>.Failure("Permission not found");
                }

                return Result<PermissionDto>.Success(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permission {PermissionId}", permissionId);
                return Result<PermissionDto>.Failure("Error retrieving permission");
            }
        }

        public async Task<IResult<PermissionDto>> CreatePermissionAsync(CreatePermissionRequest request)
        {
            try
            {
                // Check if permission already exists
                var exists = await _context.Permissions
                    .AnyAsync(p => p.Resource == request.Resource && p.Action == request.Action);

                if (exists)
                {
                    return Result<PermissionDto>.Failure($"Permission {request.Resource}.{request.Action} already exists");
                }

                var permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = $"{request.Resource}.{request.Action}",
                    Resource = request.Resource,
                    Action = request.Action,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();

                var dto = new PermissionDto
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Resource = permission.Resource,
                    Action = permission.Action,
                    Description = permission.Description,
                    CreatedAt = permission.CreatedAt
                };

                _logger.LogInformation("Created permission {PermissionName}", permission.Name);
                return Result<PermissionDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission {Resource}.{Action}", request.Resource, request.Action);
                return Result<PermissionDto>.Failure("Error creating permission");
            }
        }

        public async Task<bool> CheckUserPermissionAsync(Guid userId, Guid organizationId, string resource, string action)
        {
            try
            {
                // Check if user has the specific permission through their roles
                var hasPermission = await _context.UserOrganizationRoles
                    .Where(uor => uor.UserId == userId && uor.OrganizationId == organizationId)
                    .SelectMany(uor => uor.Role!.RolePermissions!)
                    .Select(rp => rp.Permission)
                    .AnyAsync(p => p!.Resource == resource && p.Action == action);

                return hasPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Resource}.{Action} for user {UserId}", 
                    resource, action, userId);
                return false;
            }
        }

        public async Task<IResult<List<PermissionDto>>> GetUserPermissionsAsync(Guid userId, Guid organizationId)
        {
            try
            {
                // Get all unique permissions from user's roles in this organization
                var permissions = await _context.UserOrganizationRoles
                    .Where(uor => uor.UserId == userId && uor.OrganizationId == organizationId)
                    .SelectMany(uor => uor.Role!.RolePermissions!)
                    .Select(rp => rp.Permission!)
                    .Distinct()
                    .OrderBy(p => p.Resource)
                    .ThenBy(p => p.Action)
                    .Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Resource = p.Resource,
                        Action = p.Action,
                        Description = p.Description,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                return Result<List<PermissionDto>>.Success(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions for user {UserId} in organization {OrganizationId}", 
                    userId, organizationId);
                return Result<List<PermissionDto>>.Failure("Error retrieving user permissions");
            }
        }

        public async Task<IResult<bool>> SeedDefaultPermissionsAsync()
        {
            try
            {
                // Check if permissions already exist
                var existingCount = await _context.Permissions.CountAsync();
                if (existingCount > 0)
                {
                    _logger.LogInformation("Permissions already seeded ({Count} permissions exist)", existingCount);
                    return Result<bool>.Success(true);
                }

                var permissions = new List<Permission>
                {
                    // User permissions
                    new Permission { Id = Guid.NewGuid(), Name = "users.create", Resource = "users", Action = "create", Description = "Create new users", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "users.read", Resource = "users", Action = "read", Description = "View user profiles", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "users.update", Resource = "users", Action = "update", Description = "Edit user information", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "users.delete", Resource = "users", Action = "delete", Description = "Soft delete users", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "users.hardDelete", Resource = "users", Action = "hardDelete", Description = "Permanently delete users", CreatedAt = DateTime.UtcNow },

                    // Project permissions
                    new Permission { Id = Guid.NewGuid(), Name = "projects.create", Resource = "projects", Action = "create", Description = "Create new projects", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "projects.read", Resource = "projects", Action = "read", Description = "View project details", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "projects.update", Resource = "projects", Action = "update", Description = "Edit project information", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "projects.delete", Resource = "projects", Action = "delete", Description = "Delete projects", CreatedAt = DateTime.UtcNow },

                    // Role permissions
                    new Permission { Id = Guid.NewGuid(), Name = "roles.create", Resource = "roles", Action = "create", Description = "Create new roles", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "roles.read", Resource = "roles", Action = "read", Description = "View roles and permissions", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "roles.update", Resource = "roles", Action = "update", Description = "Edit role information", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "roles.delete", Resource = "roles", Action = "delete", Description = "Delete roles", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "roles.manage", Resource = "roles", Action = "manage", Description = "Full role and permission management", CreatedAt = DateTime.UtcNow },

                    // Invitation permissions
                    new Permission { Id = Guid.NewGuid(), Name = "invitations.send", Resource = "invitations", Action = "send", Description = "Send user invitations", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "invitations.cancel", Resource = "invitations", Action = "cancel", Description = "Cancel pending invitations", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "invitations.view", Resource = "invitations", Action = "view", Description = "View organization invitations", CreatedAt = DateTime.UtcNow },

                    // Organization permissions
                    new Permission { Id = Guid.NewGuid(), Name = "organization.manage", Resource = "organization", Action = "manage", Description = "Manage organization settings", CreatedAt = DateTime.UtcNow },
                    new Permission { Id = Guid.NewGuid(), Name = "organization.view", Resource = "organization", Action = "view", Description = "View organization information", CreatedAt = DateTime.UtcNow },

                    // Admin permissions
                    new Permission { Id = Guid.NewGuid(), Name = "admin.all", Resource = "admin", Action = "all", Description = "Full administrative access", CreatedAt = DateTime.UtcNow }
                };

                _context.Permissions.AddRange(permissions);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Seeded {Count} default permissions", permissions.Count);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding default permissions");
                return Result<bool>.Failure("Error seeding permissions");
            }
        }
    }
}