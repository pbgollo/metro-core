using System.Net;

namespace Metro.Shared.Results
{
    public sealed class ApiResult<T>
    {
        public HttpStatusCode StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }

        public static ApiResult<T> Ok(T? data = default, string message = "OK")
            => new()
            {
                StatusCode = HttpStatusCode.OK,
                Message = message,
                Data = data
            };

        public static ApiResult<T> Created(T data, string message = "Resource created successfully.")
            => new()
            {
                StatusCode = HttpStatusCode.Created,
                Message = message,
                Data = data
            };

        public static ApiResult<T> NoContent(string message = "No content to return.")
            => new()
            {
                StatusCode = HttpStatusCode.NoContent,
                Message = message
            };

        public static ApiResult<T> BadRequest(string message = "Bad request.")
            => new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = message
            };

        public static ApiResult<T> NotFound(string message = "Resource not found.")
            => new()
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = message
            };

        public static ApiResult<T> Conflict(string message = "Resource conflict.")
            => new()
            {
                StatusCode = HttpStatusCode.Conflict,
                Message = message
            };

        public static ApiResult<T> Unauthorized(string message = "Unauthorized")
            => new()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Message = message
            };

        public static ApiResult<T> InternalServerError(string message = "An internal server error occurred.")
            => new()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = message
            };
    }

    public sealed class CreatedId
    {
        public Guid Id { get; }

        public CreatedId(Guid id)
        {
            Id = id;
        }
    }
}
