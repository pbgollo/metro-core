using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Application.Auth.Commands
{
    public class RequestPasswordRecoveryCommand : ICommand<ApiResult<object>>
    {
        public string? Email { get; set; }
    }
}
