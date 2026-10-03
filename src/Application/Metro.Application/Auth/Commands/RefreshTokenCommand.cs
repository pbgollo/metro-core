using Metro.Application.Auth.ViewModels;
using Metro.Application.Messaging;
using Metro.Application.Results;

namespace Metro.Application.Auth.Commands
{
    public class RefreshTokenCommand : ICommand<ApiResult<LoginViewModel>>
    {
        public string? RefreshToken { get; set; }
    }
}
