using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Authentication.Commands
{
    public class LogoutCommand : ICommand<ApiResult<object>>
    {
        public string? RefreshToken { get; set; }
    }
}
