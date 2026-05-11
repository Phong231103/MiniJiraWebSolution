namespace Web.Domain.Entities;

using System;
using Web.Domain.Common;

public class Attachment : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;

    public Guid IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public Guid UploadedById { get; set; }
    public User UploadedBy { get; set; } = null!;
}
