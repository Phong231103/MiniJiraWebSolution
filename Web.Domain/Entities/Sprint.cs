namespace Web.Domain.Entities;

using System;
using System.Collections.Generic;
using Web.Domain.Common;

public class Sprint : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
