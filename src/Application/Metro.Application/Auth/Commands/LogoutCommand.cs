using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Application.Auth.Commands
{
    public class LogoutCommand : ICommand<ApiResult<object>>
    {
        public string? RefreshToken { get; set; }
    }
}
