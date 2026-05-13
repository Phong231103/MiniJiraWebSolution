namespace Web.Application.Issues.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Common.Interfaces;
using Web.Domain.Enums;

public record UpdateIssueStatusCommand : IRequest<bool>
{
    public Guid IssueId { get; init; }
    public IssueStatus Status { get; init; }
}

public class UpdateIssueStatusCommandHandler : IRequestHandler<UpdateIssueStatusCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateIssueStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateIssueStatusCommand request, CancellationToken cancellationToken)
    {
        var issue = await _context.Issues.FirstOrDefaultAsync(i => i.Id == request.IssueId, cancellationToken);

        if (issue == null)
            return false;

        issue.Status = request.Status;
        issue.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
