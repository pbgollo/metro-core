using System.Net;

namespace Metro.Shared.Results
{
    public interface ICommandResult<T>
    {
        HttpStatusCode StatusCode { get; }
        string Message { get; }
        Guid? Id { get; }
    }
}