using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Permission
{
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePermissionRequest
    {
        [Required(ErrorMessage = "Resource is required")]
        [StringLength(50, ErrorMessage = "Resource cannot exceed 50 characters")]
        public string Resource { get; set; } = string.Empty;

        [Required(ErrorMessage = "Action is required")]
        [StringLength(50, ErrorMessage = "Action cannot exceed 50 characters")]
        public string Action { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
    }

    public class PermissionListDto
    {
        public List<PermissionDto> Permissions { get; set; } = new();
        public int TotalCount { get; set; }
    }
}