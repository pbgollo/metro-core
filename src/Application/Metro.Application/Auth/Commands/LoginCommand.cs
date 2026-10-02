using Metro.Application.Auth.ViewModels;
using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Application.Auth.Commands
{
    public class LoginCommand : ICommand<ApiResult<LoginViewModel>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
