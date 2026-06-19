using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.API.Common;
using Web.API.Response;
using Web.Application.Auth.Commands;
using Web.Application.Auth.DTOs;

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
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return id.ToActionResult<string>(HttpContext);
        }

        [HttpPost("verifyOtp")]
        [ProducesResponseType(typeof(ApiResponse<ProvisionalTokenResponse>), 200)]
        public async Task<IActionResult> VerifyOtpAndRegister([FromBody] RegisterCommand command)
        {
            var authResponse = await _mediator.Send(command);
            return authResponse.ToActionResult<ProvisionalTokenResponse>(HttpContext);
        }
    }
}
