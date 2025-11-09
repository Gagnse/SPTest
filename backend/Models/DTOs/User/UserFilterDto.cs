using System.ComponentModel.DataAnnotations;
using backend.Models.DTOs.User;

namespace backend.Models.DTOs.User
{
    public class UserFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public string? Department { get; set; }
        public bool? IsActive { get; set; }
        public bool IncludeDeleted { get; set; } = false;
        
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
        
        public string SortBy { get; set; } = "firstName";
        public bool SortDescending { get; set; } = false;
    }

    public class UserListDto
    {
        public List<UserDto> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class UserPermissionDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }
}