using MediatR;
using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Commands
{
    public class DeleteUserCommand : ICommand<ICommandResult<Unit>>
    {
        public Guid Id { get; set; }
    }
}
