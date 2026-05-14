namespace Web.Application.Issues.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Common.Interfaces;
using Web.Application.Common.Results;
using Web.Domain.Entities;
using Web.Domain.Enums;

public record CreateIssueCommand : IRequest<Guid>
{
    public string Summary { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IssueType Type { get; init; }
    public IssuePriority Priority { get; init; }
    public Guid? AssigneeId { get; init; }
    public Guid ReporterId { get; init; }
    public Guid ProjectId { get; init; }
    public Guid? SprintId { get; init; }
}

public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateIssueCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateIssueCommand request, CancellationToken cancellationToken)
    {
        // H2: Basic input validation
        if (string.IsNullOrWhiteSpace(request.Summary))
        {
            return Result<Guid>.Failure(Error.Required);
        }

        if (request.ProjectId == Guid.Empty)
        {
            return Result<Guid>.Failure(Error.Required);
        }

        if (request.ReporterId == Guid.Empty)
        {
            return Result<Guid>.Failure(Error.Required);
        }

        // Generate issue key based on project key and issue count
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return Result<Guid>.Failure(Error.NotFound);
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
            ReporterId = request.ReporterId,
            ProjectId = request.ProjectId,
            SprintId = request.SprintId,
            Status = IssueStatus.Backlog,
            CreatedAt = DateTime.UtcNow
        };

        _context.Issues.Add(issue);

        return await Task.FromResult(new Result(true, _context.SaveChangesAsync(cancellationToken)));

        return issue.Id;
    }
}
