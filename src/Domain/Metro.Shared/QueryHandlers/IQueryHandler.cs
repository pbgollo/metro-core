using MediatR;

namespace Metro.Shared.QueryHandlers
{
    public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IRequest<TResult>
    {
    }
}
