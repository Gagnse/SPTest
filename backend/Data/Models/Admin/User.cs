using System;
using System.Collections.Generic;

namespace SpaceLogic.Data.Models.Admin
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public Guid OrganizationId { get; set; }

        public string? Role { get; set; }

        public string? Department { get; set; }

        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastActive { get; set; }

        public bool IsActive { get; set; }

        // Navigation vers l'organisation
        public Organization? Organization { get; set; }

        // Navigation inverse pour les rôles, projets, etc.
        public ICollection<UserOrganizationRole>? OrganizationRoles { get; set; }

        public ICollection<ProjectUser>? ProjectAssignments { get; set; }

        public ICollection<OrganizationInvitation>? SentInvitations { get; set; }
    }
}
