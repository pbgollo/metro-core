using Metro.Domain.Users.Authentication.ViewModel;
using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Authentication.Commands
{
    public class RefreshTokenCommand : ICommand<ApiResult<LoginViewModel>>
    {
        public string? RefreshToken { get; set; }
    }
}
