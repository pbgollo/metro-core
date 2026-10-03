using MediatR;

namespace Metro.Application.Messaging
{
    public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }
}
