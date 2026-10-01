using Microsoft.AspNetCore.Mvc;
using Metro.Shared.Results;
using Metro.Shared.Returns;

namespace Metro.Application.Controllers
{
    public abstract class ApiController : ControllerBase
    {
        protected ObjectResult FromResult<T>(Return<T> result)
            => StatusCode((int)result.StatusCode, result);

        protected ObjectResult FromResult(ICommandResult<MediatR.Unit> result)
            => StatusCode((int)result.StatusCode, result);
    }
}
