// backend/Models/DTOs/Project/ProjectDto.cs
using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs.Project
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string ProjectNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "active";
        public string? Type { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? OrganizationName { get; set; }
        public Guid OrganizationId { get; set; }

        // Additional project metadata
        public IEnumerable<ProjectUserDto>? AssignedUsers { get; set; }
        public int? TotalTasks { get; set; }
        public int? CompletedTasks { get; set; }
        public decimal? CompletionPercentage { get; set; }
        public DateTime? LastActivityDate { get; set; }

        // Computed properties
        public int DaysRunning => (DateTime.UtcNow - StartDate).Days;
        public bool IsOverdue => EndDate.HasValue && DateTime.UtcNow > EndDate.Value && Status != "completed";
        public string StatusDisplayName => Status switch
        {
            "active" => "Active",
            "pending" => "Pending",
            "completed" => "Completed",
            "archive" => "Archived",
            "on_hold" => "On Hold",
            _ => Status
        };
    }

    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Project number is required")]
        [StringLength(50, ErrorMessage = "Project number cannot exceed 50 characters")]
        [RegularExpression(@"^[A-Z0-9\-_]+$", ErrorMessage = "Project number can only contain uppercase letters, numbers, hyphens, and underscores")]
        public string ProjectNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(200, ErrorMessage = "Project name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string? Type { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? ImageUrl { get; set; }

        // Validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.HasValue && StartDate.HasValue && EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "End date cannot be earlier than start date",
                    new[] { nameof(EndDate) }
                );
            }
        }
    }

    public class UpdateProjectRequest
    {
        [StringLength(200, ErrorMessage = "Project name cannot exceed 200 characters")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string? Type { get; set; }

        public string? Status { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? ImageUrl { get; set; }

        // Validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.HasValue && StartDate.HasValue && EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "End date cannot be earlier than start date",
                    new[] { nameof(EndDate) }
                );
            }

            if (!string.IsNullOrEmpty(Status))
            {
                var validStatuses = new[] { "active", "pending", "completed", "archive", "on_hold" };
                if (!validStatuses.Contains(Status.ToLower()))
                {
                    yield return new ValidationResult(
                        $"Status must be one of: {string.Join(", ", validStatuses)}",
                        new[] { nameof(Status) }
                    );
                }
            }
        }
    }

    public class ProjectSearchRequest : backend.Models.DTOs.Common.PaginationRequest
    {
        public string? Status { get; set; }
        public string? Type { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public Guid? OrganizationId { get; set; }
        public bool? IsOverdue { get; set; }
    }

    public class AssignUserToProjectRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }

        public string? Role { get; set; }
        public string? Notes { get; set; }
    }

    public class ProjectUserDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? ProjectRole { get; set; }
        public DateTime AssignedAt { get; set; }
        public string? AvatarUrl { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    public class ProjectStatsDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int OverdueProjects { get; set; }
        public int ProjectsThisMonth { get; set; }
        public decimal AverageProjectDuration { get; set; }
        public IEnumerable<ProjectTypeStatsDto> ProjectsByType { get; set; } = new List<ProjectTypeStatsDto>();
        public IEnumerable<ProjectStatusStatsDto> ProjectsByStatus { get; set; } = new List<ProjectStatusStatsDto>();
    }

    public class ProjectTypeStatsDto
    {
        public string Type { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProjectStatusStatsDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}