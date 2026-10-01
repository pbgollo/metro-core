using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Metro.Domain.Users.Authentication.Queries;

namespace Metro.Application.Controllers
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
        public async Task<ObjectResult> Login([FromBody] LoginQuery query)
            => FromResult(await _mediator.Send(query));
    }
}
