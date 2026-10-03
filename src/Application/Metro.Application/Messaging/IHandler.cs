using MediatR;
using Metro.Application.Results;

namespace Metro.Application.Messaging
{
    public interface IHandler<TCommand, TResponse> : IRequestHandler<TCommand, ApiResult<TResponse>>
        where TCommand : ICommand<ApiResult<TResponse>>
    {
    }
}
