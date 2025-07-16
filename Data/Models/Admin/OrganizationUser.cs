using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceLogic.Data.Models.Admin
{
    [Table("organization_user")] // If your table name is lowercase
    public class OrganizationUser
    {
        [Column("organization_id")]
        public Guid OrganizationId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("joined_at")]
        public DateTime JoinedAt { get; set; }

        // Navigation
        public Organization? Organization { get; set; }

        public User? User { get; set; }
    }
}