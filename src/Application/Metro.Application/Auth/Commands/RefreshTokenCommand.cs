using Metro.Application.Auth.ViewModels;
using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Application.Auth.Commands
{
    public class RefreshTokenCommand : ICommand<ApiResult<LoginViewModel>>
    {
        public string? RefreshToken { get; set; }
    }
}
