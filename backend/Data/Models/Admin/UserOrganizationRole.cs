using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceLogic.Data.Models.Admin
{
    public class UserOrganizationRole
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid RoleId { get; set; }

        public DateTime AssignedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Organization? Organization { get; set; }
        public OrganizationRole? Role { get; set; }
    }
}
