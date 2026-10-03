using Metro.Application.Messaging;
using Metro.Application.Results;

namespace Metro.Application.Users.Commands
{
    public class DeleteUserCommand : ICommand<ApiResult<object?>>
    {
        public Guid Id { get; set; }
    }
}
