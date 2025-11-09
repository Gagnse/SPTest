using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Organization
{
    public class OrganizationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? SuperAdminId { get; set; }
        public string? SuperAdminName { get; set; }

        // Organization statistics
        public int? TotalUsers { get; set; }
        public int? TotalProjects { get; set; }
        public int? ActiveProjects { get; set; }
        public DateTime? JoinedAt { get; set; } // For user-organization relationship
    }

    public class CreateOrganizationRequest
    {
        [Required(ErrorMessage = "Organization name is required")]
        [StringLength(100, ErrorMessage = "Organization name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Slug cannot exceed 50 characters")]
        [RegularExpression(@"^[a-z0-9\-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string? Slug { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Invalid website URL")]
        public string? Website { get; set; }

        public string? LogoUrl { get; set; }

        // Super admin user details
        [Required(ErrorMessage = "Super admin first name is required")]
        public string SuperAdminFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Super admin last name is required")]
        public string SuperAdminLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Super admin email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string SuperAdminEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Super admin password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string SuperAdminPassword { get; set; } = string.Empty;
    }

    public class UpdateOrganizationRequest
    {
        [StringLength(100, ErrorMessage = "Organization name cannot exceed 100 characters")]
        public string? Name { get; set; }

        [StringLength(50, ErrorMessage = "Slug cannot exceed 50 characters")]
        [RegularExpression(@"^[a-z0-9\-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string? Slug { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Invalid website URL")]
        public string? Website { get; set; }

        public string? LogoUrl { get; set; }
    }

    public class OrganizationInvitationDto
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string Status { get; set; } = string.Empty; // pending, accepted, expired, cancelled
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
        public bool IsPending => Status.Equals("pending", StringComparison.OrdinalIgnoreCase);
    }

    public class InviteUserToOrganizationRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        public string? Role { get; set; }

        public string? Message { get; set; }

        [Range(1, 30, ErrorMessage = "Expiry days must be between 1 and 30")]
        public int ExpiryDays { get; set; } = 7;

        // ⭐ ADDED: Department and Location fields
        [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
        public string? Department { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string? Location { get; set; }
    }

    public class AcceptInvitationRequest
    {
        [Required(ErrorMessage = "Invitation token is required")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class OrganizationStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }
}