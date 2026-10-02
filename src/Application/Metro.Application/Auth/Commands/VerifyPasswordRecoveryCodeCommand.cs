using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Application.Auth.Commands
{
    public class VerifyPasswordRecoveryCodeCommand : ICommand<ApiResult<object>>
    {
        public string? Email { get; set; }
        public string? Code { get; set; }
    }
}
