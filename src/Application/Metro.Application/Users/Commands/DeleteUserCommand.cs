using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Application.Users.Commands
{
    public class DeleteUserCommand : ICommand<ApiResult<object?>>
    {
        public Guid Id { get; set; }
    }
}
