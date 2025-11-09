using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Models.Admin;

namespace SpaceLogic.Data.Admin
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Organization> Organizations { get; set; } = null!;
        public DbSet<OrganizationRole> OrganizationRoles { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<OrganizationInvitation> OrganizationInvitations { get; set; } = null!;
        public DbSet<OrganizationUser> OrganizationUsers { get; set; } = null!;
        public DbSet<UserOrganizationRole> UserOrganizationRoles { get; set; } = null!;
        public DbSet<ProjectUser> ProjectUsers { get; set; } = null!;
        
        // ⭐ NEW DbSets
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public DbSet<EmailToken> EmailTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table name mapping (PostgreSQL lowercase convention)
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Organization>().ToTable("organizations");
            modelBuilder.Entity<OrganizationRole>().ToTable("organization_roles");
            modelBuilder.Entity<Project>().ToTable("projects");
            modelBuilder.Entity<OrganizationInvitation>().ToTable("organization_invitations");
            modelBuilder.Entity<OrganizationUser>().ToTable("organization_user");
            modelBuilder.Entity<UserOrganizationRole>().ToTable("user_organization_role");
            modelBuilder.Entity<ProjectUser>().ToTable("project_users");
            modelBuilder.Entity<Permission>().ToTable("permissions");
            modelBuilder.Entity<RolePermission>().ToTable("role_permissions");
            modelBuilder.Entity<EmailToken>().ToTable("email_tokens");

            // ⭐ SOFT DELETE QUERY FILTER
            // Automatically exclude soft-deleted users from all queries
            modelBuilder.Entity<User>().HasQueryFilter(u => u.DeletedAt == null);

            // Composite Keys
            modelBuilder.Entity<OrganizationUser>()
                .HasKey(ou => new { ou.OrganizationId, ou.UserId });

            modelBuilder.Entity<UserOrganizationRole>()
                .HasKey(ur => new { ur.UserId, ur.OrganizationId, ur.RoleId });

            modelBuilder.Entity<ProjectUser>()
                .HasKey(pu => new { pu.ProjectId, pu.UserId });

            // ⭐ NEW Composite Key for RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // ⭐ NEW Unique Constraint on Permission (resource + action combination)
            modelBuilder.Entity<Permission>()
                .HasIndex(p => new { p.Resource, p.Action })
                .IsUnique();

            // ⭐ NEW Unique Constraint on EmailToken (token must be unique)
            modelBuilder.Entity<EmailToken>()
                .HasIndex(et => et.Token)
                .IsUnique();

            // Organization.SuperAdmin relationship
            modelBuilder.Entity<Organization>()
                .HasOne(o => o.SuperAdmin)
                .WithMany()
                .HasForeignKey(o => o.SuperAdminId)
                .OnDelete(DeleteBehavior.SetNull);

            // ⭐ NEW Permission Relationships
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RoleAssignments)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ⭐ NEW EmailToken Relationships
            modelBuilder.Entity<EmailToken>()
                .HasOne(et => et.User)
                .WithMany(u => u.EmailTokens)
                .HasForeignKey(et => et.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}