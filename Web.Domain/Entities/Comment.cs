namespace Web.Domain.Entities;

using System;
using Web.Domain.Common;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;

    public Guid IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
