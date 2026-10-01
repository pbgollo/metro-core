using Metro.Domain.Users.ViewModel;
using Metro.Shared.Queries;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Queries
{
    public class GetUserQuery : IQuery<ApiResult<GetUserViewModel>>
    {
        public Guid Id { get; set; }
    }
}
