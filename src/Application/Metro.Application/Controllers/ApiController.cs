using Microsoft.AspNetCore.Mvc;
using Metro.Shared.Results;

namespace Metro.Application.Controllers
{
    public abstract class ApiController : ControllerBase
    {
        protected ObjectResult FromResult<T>(ApiResult<T> result)
            => StatusCode((int)result.StatusCode, result);
    }
}
