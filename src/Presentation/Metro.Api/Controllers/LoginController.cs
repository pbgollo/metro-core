using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Metro.Application.Auth.Commands;

namespace Metro.Api.Controllers
{
    [ApiController]
    [Route("api/v1/login")]
    [ApiExplorerSettings(GroupName = "ApiV1")]
    public class LoginController : ApiController
    {
        private readonly IMediator _mediator;
        public LoginController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [EnableRateLimiting("login")]
        public async Task<ObjectResult> Login([FromBody] LoginCommand command)
            => FromResult(await _mediator.Send(command));

        [HttpPost("refresh")]
        [EnableRateLimiting("login")]
        public async Task<ObjectResult> Refresh([FromBody] RefreshTokenCommand command)
            => FromResult(await _mediator.Send(command));

        [HttpPost("logout")]
        public async Task<ObjectResult> Logout([FromBody] LogoutCommand command)
            => FromResult(await _mediator.Send(command));
    }
}
