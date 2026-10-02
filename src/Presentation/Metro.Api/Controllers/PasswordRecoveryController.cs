using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Metro.Application.Auth.Commands;

namespace Metro.Api.Controllers
{
    [ApiController]
    [Route("api/v1/password-recovery")]
    [ApiExplorerSettings(GroupName = "ApiV1")]
    public class PasswordRecoveryController : ApiController
    {
        private readonly IMediator _mediator;

        public PasswordRecoveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("request")]
        [EnableRateLimiting("password-recovery")]
        public async Task<ObjectResult> RequestCode([FromBody] RequestPasswordRecoveryCommand command)
            => FromResult(await _mediator.Send(command));

        [HttpPost("verify")]
        [EnableRateLimiting("password-recovery")]
        public async Task<ObjectResult> Verify([FromBody] VerifyPasswordRecoveryCodeCommand command)
            => FromResult(await _mediator.Send(command));

        [HttpPost("reset")]
        [EnableRateLimiting("password-recovery")]
        public async Task<ObjectResult> Reset([FromBody] ResetPasswordCommand command)
            => FromResult(await _mediator.Send(command));
    }
}
