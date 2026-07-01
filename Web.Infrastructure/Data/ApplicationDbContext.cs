namespace Web.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Web.Application.Common.Interfaces;
using Web.Domain.Entities;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Project -> Owner
        builder.Entity<Project>()
            .HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Project -> Sprints
        builder.Entity<Project>()
            .HasMany(p => p.Sprints)
            .WithOne(s => s.Project)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project -> Issues
        builder.Entity<Project>()
            .HasMany(p => p.Issues)
            .WithOne(i => i.Project)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Sprint -> Issues
        builder.Entity<Sprint>()
            .HasMany(s => s.Issues)
            .WithOne(i => i.Sprint)
            .HasForeignKey(i => i.SprintId)
            .OnDelete(DeleteBehavior.SetNull);

        // Issue -> Assignee
        builder.Entity<Issue>()
            .HasOne(i => i.Assignee)
            .WithMany()
            .HasForeignKey(i => i.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        // Issue -> Reporter
        builder.Entity<Issue>()
            .HasOne(i => i.Reporter)
            .WithMany()
            .HasForeignKey(i => i.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Issue -> Comments
        builder.Entity<Issue>()
            .HasMany(i => i.Comments)
            .WithOne(c => c.Issue)
            .HasForeignKey(c => c.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Issue -> Attachments
        builder.Entity<Issue>()
            .HasMany(i => i.Attachments)
            .WithOne(a => a.Issue)
            .HasForeignKey(a => a.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Roles (many-to-many)
        builder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Name)
                  .IsRequired()
                  .HasMaxLength(256);

            entity.Property(r => r.DisplayName)
                  .IsRequired()
                  .HasMaxLength(256);

            entity.HasIndex(r => r.Name).IsUnique();

            entity.HasMany(r => r.Permissions)
                 .WithMany(p => p.Roles)
                 .UsingEntity<Dictionary<string, object>>(
                       "RolePermission",
                       j => j.HasOne<Permission>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade),
                       j => j.HasOne<Role>().WithMany().HasForeignKey("PermissionId").OnDelete(DeleteBehavior.Cascade),
                       j =>
                       {
                           j.HasKey("PermissionId", "RoleId");
                           j.ToTable("PermissionRoles");
                       });
        });

        builder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(256);

            entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

            entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(256);

            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasMany(u => u.Devices)
                  .WithOne(d => d.User)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.RefreshTokens)
                  .WithOne(rt => rt.User)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Roles)
                  .WithMany(r => r.Users)
                  .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");
                            j.ToTable("UserRoles");
                        });
        });

        builder.Entity<Device>(entity =>
        {
            entity.HasMany(u => u.RefreshTokens)
                  .WithOne(d => d.Device)
                  .HasForeignKey(d => d.DeviceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
