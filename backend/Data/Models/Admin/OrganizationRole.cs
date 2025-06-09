using System;
using System.Collections.Generic;

namespace SpaceLogic.Data.Models.Admin
{
    public class OrganizationRole
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public Organization? Organization { get; set; }

        public ICollection<UserOrganizationRole>? UserAssignments { get; set; }

        public ICollection<ProjectUser>? ProjectAssignments { get; set; }
    }
}
