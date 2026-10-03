using Metro.Application.Messaging;
using Metro.Application.Results;

namespace Metro.Application.Auth.Commands
{
    public class RequestPasswordRecoveryCommand : ICommand<ApiResult<object>>
    {
        public string? Email { get; set; }
    }
}
