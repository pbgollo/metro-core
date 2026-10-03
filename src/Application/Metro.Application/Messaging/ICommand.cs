using MediatR;

namespace Metro.Application.Messaging
{
    public interface ICommand<T> : IRequest<T>
    {
    }
}
