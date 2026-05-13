namespace Web.Application.Issues.Queries.GetProjectBacklog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Common.Interfaces;
using Web.Application.Issues.DTOs;

public record GetProjectBacklogQuery(Guid ProjectId) : IRequest<List<IssueDto>>;

public class GetProjectBacklogQueryHandler : IRequestHandler<GetProjectBacklogQuery, List<IssueDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProjectBacklogQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<IssueDto>> Handle(GetProjectBacklogQuery request, CancellationToken cancellationToken)
    {
        var listIssueDto = await _context.Issues
            .Where(i => i.ProjectId == request.ProjectId && i.SprintId == null)
            .OrderByDescending(i => i.Priority)
            .ThenByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);

        var listIssueDtoMapped = _mapper.Map<List<IssueDto>>(listIssueDto);

        return listIssueDtoMapped;
    }
}
