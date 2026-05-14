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
    public DbSet<Roles> Roles => Set<Roles>();

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
    }
}
