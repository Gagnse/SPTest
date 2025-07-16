// backend/Models/DTOs/User/UserDto.cs
using System.ComponentModel.DataAnnotations;
using backend.Models.DTOs.Organization;

namespace backend.Models.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public Guid OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Computed properties
        public string FullName => $"{FirstName} {LastName}";
        public string Initials => $"{FirstName.FirstOrDefault()}{LastName.FirstOrDefault()}".ToUpper();
    }

    public class CreateUserRequest
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;

        public string? Role { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Organization ID is required")]
        public Guid OrganizationId { get; set; }
    }

    public class UpdateUserRequest
    {
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        public string? Role { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        public string? AvatarUrl { get; set; }
    }

    public class UserSearchRequest : backend.Models.DTOs.Common.PaginationRequest
    {
        public string? Department { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public Guid? OrganizationId { get; set; }
    }

    public class SwitchOrganizationRequest
    {
        [Required(ErrorMessage = "Organization ID is required")]
        public Guid OrganizationId { get; set; }
    }

    public class SwitchOrganizationResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
        public OrganizationDto Organization { get; set; } = null!;
    }

    public class UserProfileUpdateRequest
    {
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        public string? AvatarUrl { get; set; }
    }
}