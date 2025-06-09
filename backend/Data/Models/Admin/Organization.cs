using System;
using System.Collections.Generic;

namespace SpaceLogic.Data.Models.Admin
{
    public class Organization
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid? SuperAdminId { get; set; }

        // Navigation vers le super admin (User)
        public User? SuperAdmin { get; set; }

        // Navigation vers les autres entités liées
        public ICollection<User>? Users { get; set; }
        public ICollection<Project>? Projects { get; set; }
        public ICollection<OrganizationRole>? Roles { get; set; }
        public ICollection<OrganizationInvitation>? Invitations { get; set; }
    }
}
