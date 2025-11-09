using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceLogic.Data.Models.Admin
{
    [Table("role_permissions")]
    public class RolePermission
    {
        [Column("role_id")]
        public Guid RoleId { get; set; }

        [Column("permission_id")]
        public Guid PermissionId { get; set; }

        [Column("granted_at")]
        public DateTime GrantedAt { get; set; }

        // Navigation properties
        public OrganizationRole? Role { get; set; }
        public Permission? Permission { get; set; }
    }
}