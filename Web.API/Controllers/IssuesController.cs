namespace Web.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using Web.Application.Issues.Queries.GetProjectBacklog;
using Web.Application.Issues.Queries.GetSprintBoard;
using Web.Application.Issues.Commands;

[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IssuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateIssueCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(Create), new { id }, id);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateIssueStatusCommand command)
    {
        if (id != command.IssueId)
            return BadRequest(new { error = "Route ID and body IssueId must match." });

        var result = await _mediator.Send(command);
        if (!result) return NotFound(new { error = "Issue not found." });

        return NoContent();
    }

    [HttpGet("project/{projectId}/backlog")]
    public async Task<IActionResult> GetProjectBacklog(Guid projectId)
    {
        var issues = await _mediator.Send(new GetProjectBacklogQuery(projectId));
        return Ok(issues);
    }

    [HttpGet("sprint/{sprintId}/board")]
    public async Task<IActionResult> GetSprintBoard(Guid sprintId)
    {
        var issues = await _mediator.Send(new GetSprintBoardQuery(sprintId));
        return Ok(issues);
    }
}
