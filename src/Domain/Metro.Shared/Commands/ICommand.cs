using System;
using MediatR;

namespace Metro.Shared.Commands
{
    public interface ICommand<T> : IRequest<T>
    {

    }
}
