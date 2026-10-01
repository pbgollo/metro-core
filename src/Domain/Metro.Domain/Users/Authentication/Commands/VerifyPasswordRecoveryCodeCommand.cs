using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Authentication.Commands
{
    public class VerifyPasswordRecoveryCodeCommand : ICommand<ApiResult<object>>
    {
        public string? Email { get; set; }
        public string? Code { get; set; }
    }
}
