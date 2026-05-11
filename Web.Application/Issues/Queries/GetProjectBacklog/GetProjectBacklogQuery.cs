namespace Web.Application.Issues.Queries.GetProjectBacklog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Common.Interfaces;
using Web.Application.Issues.DTOs;

public record GetProjectBacklogQuery(Guid ProjectId) : IRequest<List<IssueDto>>;

public class GetProjectBacklogQueryHandler : IRequestHandler<GetProjectBacklogQuery, List<IssueDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectBacklogQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IssueDto>> Handle(GetProjectBacklogQuery request, CancellationToken cancellationToken)
    {
        return await _context.Issues
            .Where(i => i.ProjectId == request.ProjectId && i.SprintId == null)
            .OrderByDescending(i => i.Priority)
            .ThenByDescending(i => i.CreatedAt)
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
