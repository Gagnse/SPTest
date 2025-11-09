using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceLogic.Data.Models.Admin
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; } = null!;

        [Column("last_name")]
        public string LastName { get; set; } = null!;

        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("organization_id")]
        public Guid OrganizationId { get; set; }

        [Column("org_role")]
        public string? OrgRole { get; set; }

        [Column("department")]
        public string? Department { get; set; }

        [Column("location")]
        public string? Location { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("last_active")]
        public DateTime LastActive { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public Organization? Organization { get; set; }
        public ICollection<UserOrganizationRole>? OrganizationRoles { get; set; }
        public ICollection<ProjectUser>? ProjectAssignments { get; set; }
        public ICollection<OrganizationInvitation>? SentInvitations { get; set; }
        public ICollection<EmailToken>? EmailTokens { get; set; }

        // Computed properties
        public bool IsDeleted => DeletedAt.HasValue;
    }
}