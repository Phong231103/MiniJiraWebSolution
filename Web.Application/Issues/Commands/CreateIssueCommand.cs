namespace Web.Application.Issues.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Common.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Domain.Primitives;

public record CreateIssueCommand : IRequest<Result<bool>>
{
    public string Summary { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IssueType Type { get; init; }
    public IssuePriority Priority { get; init; }
    public Guid? AssigneeId { get; init; }
    public Guid? SprintId { get; init; }
}

public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CreateIssueCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CreateIssueCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Summary))
        {
            //return Error.NotFound "Summary is REQUIRED"));
            return Error.Required("Summary is Required", "Summary is Required.");
        }

        // Generate issue key based on project key and issue count
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return Error.NotFound("project NotFound", "project is NotFound.");
        }

        var issueCount = await _context.Issues.CountAsync(i => i.ProjectId == request.ProjectId, cancellationToken);

        var issueKey = $"{project.Key}-{issueCount + 1}";

        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            Key = issueKey,
            Summary = request.Summary,
            Description = request.Description,
            Type = request.Type,
            Priority = request.Priority,
            AssigneeId = request.AssigneeId,
            SprintId = request.SprintId,
            Status = IssueStatus.Backlog,
            CreatedAt = DateTime.UtcNow
        };

        _context.Issues.Add(issue);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
