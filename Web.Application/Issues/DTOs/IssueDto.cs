namespace Web.Application.Issues.DTOs;

using System;
using Web.Domain.Enums;

public class IssueDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IssueType Type { get; set; }
    public IssueStatus Status { get; set; }
    public IssuePriority Priority { get; set; }
    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public string? AssigneeAvatar { get; set; }
    public Guid ReporterId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SprintId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
