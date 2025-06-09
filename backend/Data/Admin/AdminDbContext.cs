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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Correspondance explicite avec les noms de tables PostgreSQL (minuscules)
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Organization>().ToTable("organizations");
            modelBuilder.Entity<OrganizationRole>().ToTable("organization_roles");
            modelBuilder.Entity<Project>().ToTable("projects");
            modelBuilder.Entity<OrganizationInvitation>().ToTable("organization_invitations");
            modelBuilder.Entity<OrganizationUser>().ToTable("organization_user");
            modelBuilder.Entity<UserOrganizationRole>().ToTable("user_organization_role");
            modelBuilder.Entity<ProjectUser>().ToTable("project_users");

            // Clés composées
            modelBuilder.Entity<OrganizationUser>()
                .HasKey(ou => new { ou.OrganizationId, ou.UserId });

            modelBuilder.Entity<UserOrganizationRole>()
                .HasKey(ur => new { ur.UserId, ur.OrganizationId, ur.RoleId });

            modelBuilder.Entity<ProjectUser>()
                .HasKey(pu => new { pu.ProjectId, pu.UserId });

            // Relation explicite : Organization.SuperAdmin → User
            modelBuilder.Entity<Organization>()
                .HasOne(o => o.SuperAdmin)
                .WithMany()
                .HasForeignKey(o => o.SuperAdminId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}