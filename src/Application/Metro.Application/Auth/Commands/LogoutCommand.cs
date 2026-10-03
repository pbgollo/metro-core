using Metro.Application.Messaging;
using Metro.Application.Results;

namespace Metro.Application.Auth.Commands
{
    public class LogoutCommand : ICommand<ApiResult<object>>
    {
        public string? RefreshToken { get; set; }
    }
}
