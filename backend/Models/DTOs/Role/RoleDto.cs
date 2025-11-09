using System;
using System.ComponentModel.DataAnnotations;
using backend.Models.DTOs.Permission;

namespace backend.Models.DTOs.Role
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserCount { get; set; }
    }

    public class RoleWithPermissionsDto
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();
        public int UserCount { get; set; }
    }

    public class CreateRoleRequest
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }

        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class UpdateRoleRequest
    {
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
    }

    public class UpdateRolePermissionsRequest
    {
        [Required(ErrorMessage = "Permission IDs are required")]
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class AssignRoleRequest
    {
        [Required(ErrorMessage = "Role ID is required")]
        public Guid RoleId { get; set; }
    }

    public class RoleListDto
    {
        public List<RoleDto> Roles { get; set; } = new();
        public int TotalCount { get; set; }
    }
}