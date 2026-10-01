using MediatR;

namespace Metro.Shared.Queries
{
    public interface IQuery<T> : IRequest<T>
    {}
}
