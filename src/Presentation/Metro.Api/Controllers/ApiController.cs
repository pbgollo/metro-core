using Microsoft.AspNetCore.Mvc;
using Metro.Shared.Results;

namespace Metro.Api.Controllers
{
    public abstract class ApiController : ControllerBase
    {
        protected ObjectResult FromResult<T>(ApiResult<T> result)
            => StatusCode(ToHttpStatusCode(result.Status), result);

        private static int ToHttpStatusCode(ResultStatus status) => status switch
        {
            ResultStatus.Ok => StatusCodes.Status200OK,
            ResultStatus.Created => StatusCodes.Status201Created,
            ResultStatus.NoContent => StatusCodes.Status204NoContent,
            ResultStatus.BadRequest => StatusCodes.Status400BadRequest,
            ResultStatus.Unauthorized => StatusCodes.Status401Unauthorized,
            ResultStatus.NotFound => StatusCodes.Status404NotFound,
            ResultStatus.Conflict => StatusCodes.Status409Conflict,
            ResultStatus.InternalError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
