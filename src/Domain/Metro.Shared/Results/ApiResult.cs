namespace Metro.Shared.Results
{
    public sealed class ApiResult<T>
    {
        public ResultStatus Status { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }

        public static ApiResult<T> Ok(T? data = default, string message = "OK")
            => new()
            {
                Status = ResultStatus.Ok,
                Message = message,
                Data = data
            };

        public static ApiResult<T> Created(T data, string message = "Resource created successfully.")
            => new()
            {
                Status = ResultStatus.Created,
                Message = message,
                Data = data
            };

        public static ApiResult<T> NoContent(string message = "No content to return.")
            => new()
            {
                Status = ResultStatus.NoContent,
                Message = message
            };

        public static ApiResult<T> BadRequest(string message = "Bad request.")
            => new()
            {
                Status = ResultStatus.BadRequest,
                Message = message
            };

        public static ApiResult<T> NotFound(string message = "Resource not found.")
            => new()
            {
                Status = ResultStatus.NotFound,
                Message = message
            };

        public static ApiResult<T> Conflict(string message = "Resource conflict.")
            => new()
            {
                Status = ResultStatus.Conflict,
                Message = message
            };

        public static ApiResult<T> Unauthorized(string message = "Unauthorized")
            => new()
            {
                Status = ResultStatus.Unauthorized,
                Message = message
            };

        public static ApiResult<T> InternalError(string message = "An internal server error occurred.")
            => new()
            {
                Status = ResultStatus.InternalError,
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
