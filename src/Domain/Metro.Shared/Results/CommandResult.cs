using System.Net;
using System.Runtime.InteropServices;
using MediatR;

namespace Metro.Shared.Results
{
    public class CommandResult : ICommandResult<Unit>
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string? Message { get; private set; }
        public Guid? Id { get; private set; }

        public CommandResult(HttpStatusCode statusCode, string? message, Guid? id = null)
        {
            StatusCode = statusCode;
            Message = message;
            Id = id;
        }

        public static ICommandResult<Unit> OK()
        {
            return new CommandResult(
                statusCode: HttpStatusCode.OK,
                message: "Request processed successfully."
            );
        }

        public static ICommandResult<Unit> Created(Guid? id = null)
        {
            if (id is not null) {
                return new CommandResult(
                    statusCode: HttpStatusCode.Created,
                    message: "Resource created successfully.",
                    id: id
                );
            } else {
                return new CommandResult(
                    statusCode: HttpStatusCode.Created,
                    message: "Resource created successfully."
                );
            }
        }

        public static ICommandResult<Unit> NoContent()
        {
            return new CommandResult(
                statusCode: HttpStatusCode.NoContent,
                message: "No content to return."
            );
        }

        public static ICommandResult<Unit> BadRequest()
        {
            return new CommandResult(
                statusCode: HttpStatusCode.BadRequest,
                message: "Bad request."
            );
        }

        public static ICommandResult<Unit> NotFound()
        {
            return new CommandResult(
                statusCode: HttpStatusCode.NotFound,
                message: "Resource not found."
            );
        }

        public static ICommandResult<Unit> Conflict()
        {
            return new CommandResult(
                statusCode: HttpStatusCode.Conflict,
                message: "Resource conflict."
            );
        }

        public static ICommandResult<Unit> InternalServerError()
        {
            return new CommandResult(
                statusCode: HttpStatusCode.InternalServerError,
                message: "An internal server error occurred."
            );
        }
    }
}
