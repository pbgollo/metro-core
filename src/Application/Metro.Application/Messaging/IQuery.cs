using MediatR;

namespace Metro.Application.Messaging
{
    public interface IQuery<T> : IRequest<T>
    {
    }
}
