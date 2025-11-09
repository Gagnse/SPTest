using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceLogic.Data.Models.Admin
{
    [Table("permissions")]
    public class Permission
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!; // e.g., "users.delete"

        [Column("resource")]
        public string Resource { get; set; } = null!; // e.g., "users"

        [Column("action")]
        public string Action { get; set; } = null!; // e.g., "delete"

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<RolePermission>? RoleAssignments { get; set; }
    }
}