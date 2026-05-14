namespace Web.Application.Common.Interfaces;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Project> Projects { get; }
    DbSet<Sprint> Sprints { get; }
    DbSet<Issue> Issues { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<Roles> Roles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
