namespace Web.Domain.Entities;

using System.Collections.Generic;
using Web.Domain.Common;

public class Project : BaseEntity
{
    public string Key { get; set; } = string.Empty; // e.g., JIRA
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
