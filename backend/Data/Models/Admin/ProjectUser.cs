using System;

namespace SpaceLogic.Data.Models.Admin
{
    public class ProjectUser
    {
        public Guid ProjectId { get; set; }

        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }

        public DateTime JoinedAt { get; set; }

        // Navigation
        public Project? Project { get; set; }

        public User? User { get; set; }

        public OrganizationRole? Role { get; set; }
    }
}
