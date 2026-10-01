using MediatR;

using Metro.Shared.Results;

namespace Metro.Shared.Handlers
{
    public interface IHandler<T> : IRequestHandler<T, ICommandResult<Unit>> where T : IRequest<ICommandResult<Unit>>
    {
        Task<ICommandResult<Unit>> Handle(T request, CancellationToken cancellationToken);
    }
}