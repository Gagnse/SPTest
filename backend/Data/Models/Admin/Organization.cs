using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceLogic.Data.Models.Admin
{
    [Table("organizations")]
    public class Organization
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("slug")]
        public string? Slug { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("logo_url")]
        public string? LogoUrl { get; set; }

        [Column("website")]
        public string? Website { get; set; }

        [Column("settings")]
        public string? Settings { get; set; } // JSON string ou JSONB?

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("super_admin_id")]
        public Guid? SuperAdminId { get; set; }

        [Column("created_by")]
        public Guid? CreatedBy { get; set; }

        // Navigation vers le super admin (User)
        public User? SuperAdmin { get; set; }

        // Navigation vers les autres entités liées
        public ICollection<User>? Users { get; set; }
        public ICollection<Project>? Projects { get; set; }
        public ICollection<OrganizationRole>? Roles { get; set; }
        public ICollection<OrganizationInvitation>? Invitations { get; set; }
    }
}
