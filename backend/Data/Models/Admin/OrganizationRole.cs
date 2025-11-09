using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceLogic.Data.Models.Admin
{
    [Table("organization_roles")]
    public class OrganizationRole
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("organization_id")]
        public Guid OrganizationId { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Organization? Organization { get; set; }
        public ICollection<UserOrganizationRole>? UserAssignments { get; set; }
        public ICollection<ProjectUser>? ProjectAssignments { get; set; }
        public ICollection<RolePermission>? RolePermissions { get; set; } // ⭐ NEW
    }
}