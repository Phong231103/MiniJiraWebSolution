namespace Web.Application.Issues.Queries.GetSprintBoard;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Common.Interfaces;
using Web.Application.Issues.DTOs;

public record GetSprintBoardQuery(Guid SprintId) : IRequest<List<IssueDto>>;

public class GetSprintBoardQueryHandler : IRequestHandler<GetSprintBoardQuery, List<IssueDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSprintBoardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IssueDto>> Handle(GetSprintBoardQuery request, CancellationToken cancellationToken)
    {
        return await _context.Issues
            .Where(i => i.SprintId == request.SprintId)
            .OrderByDescending(i => i.Priority)
            .Select(i => new IssueDto
            {
                Id = i.Id,
                Key = i.Key,
                Summary = i.Summary,
                Description = i.Description,
                Type = i.Type,
                Status = i.Status,
                Priority = i.Priority,
                AssigneeId = i.AssigneeId,
                AssigneeName = i.Assignee != null ? i.Assignee.FullName : null,
                AssigneeAvatar = i.Assignee != null ? i.Assignee.AvatarUrl : null,
                ReporterId = i.ReporterId,
                ProjectId = i.ProjectId,
                SprintId = i.SprintId,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
