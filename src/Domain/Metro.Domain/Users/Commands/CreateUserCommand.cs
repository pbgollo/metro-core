using MediatR;
using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Commands
{
    public class CreateUserCommand : ICommand<ICommandResult<Unit>>
    {
        required public string Name { get; set; }

        required public string Email { get; set; }

        required public string Document { get; set; }

        required public string Phone { get; set; }

        required public string Password { get; set; }

        public string Role { get; set; } = "client";
    }
}
