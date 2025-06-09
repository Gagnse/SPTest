using System;

namespace SpaceLogic.Data.Models.Admin
{
    public class OrganizationUser
    {
        public Guid OrganizationId { get; set; }

        public Guid UserId { get; set; }

        public DateTime JoinedAt { get; set; }

        // Navigation
        public Organization? Organization { get; set; }

        public User? User { get; set; }
    }
}
