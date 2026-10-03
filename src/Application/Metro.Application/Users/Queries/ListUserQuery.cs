using Metro.Application.Users.ViewModels;
using Metro.Application.Messaging;
using Metro.Application.Results;

namespace Metro.Application.Users.Queries
{
    public class ListUserQuery : IQuery<ApiResult<ListUserResponse>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
    }
}
