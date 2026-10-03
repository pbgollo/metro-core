using Metro.Application.Messaging;
using Metro.Application.Results;

namespace Metro.Application.Auth.Commands
{
    public class ResetPasswordCommand : ICommand<ApiResult<object>>
    {
        public string? Email { get; set; }
        public string? Code { get; set; }
        public string? NewPassword { get; set; }
    }
}
