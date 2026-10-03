using Metro.Application.Users.ViewModels;
using Metro.Application.Messaging;
using Metro.Application.Results;

namespace Metro.Application.Users.Queries
{
    public class GetUserQuery : IQuery<ApiResult<GetUserViewModel>>
    {
        public Guid Id { get; set; }
    }
}
