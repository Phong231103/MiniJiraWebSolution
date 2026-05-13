namespace Web.Application.Issues.Queries.GetSprintBoard;

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

public record GetSprintBoardQuery(Guid SprintId) : IRequest<List<IssueDto>>;

public class GetSprintBoardQueryHandler : IRequestHandler<GetSprintBoardQuery, List<IssueDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSprintBoardQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<IssueDto>> Handle(GetSprintBoardQuery request, CancellationToken cancellationToken)
    {
        var listIssueDto = await _context.Issues
            .Where(i => i.SprintId == request.SprintId)
            .OrderByDescending(i => i.Priority)
            .ToListAsync(cancellationToken);

        var listIssueDtoMapped = _mapper.Map<List<IssueDto>>(listIssueDto);

        return listIssueDtoMapped;
    }
}
