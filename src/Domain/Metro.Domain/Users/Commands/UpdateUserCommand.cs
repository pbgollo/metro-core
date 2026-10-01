using MediatR;
using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Commands
{
    public class UpdateUserCommand : ICommand<ICommandResult<Unit>>
    {
        public Guid Id { get; set; }

        required public string Name { get; set; }

        required public string Email { get; set; }

        required public string Document { get; set; }

        required public string Phone { get; set; }

        public string? Password { get; set; }

        public string Role { get; set; } = "client";

        public bool? IsActive { get; set; }
    }
}
