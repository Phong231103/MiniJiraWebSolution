using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Common;
using Web.Application.Issues.Commands;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("firstTimeRegistration")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Register([FromBody] CreateIssueCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), new { id }, id);
        }
    }
}
