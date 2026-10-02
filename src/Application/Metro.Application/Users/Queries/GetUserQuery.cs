using Metro.Application.Users.ViewModels;
using Metro.Shared.Queries;
using Metro.Shared.Results;

namespace Metro.Application.Users.Queries
{
    public class GetUserQuery : IQuery<ApiResult<GetUserViewModel>>
    {
        public Guid Id { get; set; }
    }
}
