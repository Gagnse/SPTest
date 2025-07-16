using System;

namespace SpaceLogic.Data.Models.Admin
{
    public class Project
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public string ProjectNumber { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Status { get; set; }

        public string? Type { get; set; }

        public string? ImageUrl { get; set; }

        // Navigation
        public Organization? Organization { get; set; }

        public ICollection<ProjectUser>? AssignedUsers { get; set; }
    }
}
