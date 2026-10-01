using MediatR;
using Metro.Shared.Results;

namespace Metro.Shared.Handlers
{
    public interface IHandler<TCommand, TResponse> : IRequestHandler<TCommand, ApiResult<TResponse>>
        where TCommand : IRequest<ApiResult<TResponse>>
    {
    }
}
