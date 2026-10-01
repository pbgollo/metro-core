using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Metro.Domain.Users.Commands;
using Metro.Domain.Users.Queries;
using Metro.Domain.Users.ViewModel;
using Metro.Shared.Returns;

namespace Metro.Application.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [ApiExplorerSettings(GroupName = "ApiV1")]
    public class UserController : ApiController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        [Authorize("user")]
        public async Task<ObjectResult> Me()
        {
            var idValue = User.FindFirstValue(ClaimTypes.Name);
            if (!Guid.TryParse(idValue, out var id))
            {
                return FromResult(Return<GetUserViewModel>.Unauthorized(new GetUserViewModel()));
            }

            return FromResult(await _mediator.Send(new GetUserQuery
            {
                Id = id
            }));
        }

        [HttpGet]
        [Authorize("master")]
        public async Task<ObjectResult> List([FromQuery] ListUserQuery query)
            => FromResult(await _mediator.Send(query));

        [HttpGet("{id}")]
        [Authorize("master")]
        public async Task<ObjectResult> Get(Guid id)
            => FromResult(await _mediator.Send(new GetUserQuery
            {
                Id = id
            }));

        [HttpPost]
        [EnableRateLimiting("create-user")]
        public async Task<ObjectResult> Create([FromBody] CreateUserCommand command)
        {
            if (!User.IsInRole("master"))
            {
                command.Role = "client";
            }
            else if (command.Role is not ("master" or "client"))
            {
                command.Role = "client";
            }

            return FromResult(await _mediator.Send(command));
        }

        [HttpPut("{id}")]
        [Authorize("master")]
        public async Task<ObjectResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            return FromResult(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize("master")]
        public async Task<ObjectResult> Delete(Guid id)
            => FromResult(await _mediator.Send(new DeleteUserCommand
            {
                Id = id
            }));
    }
}