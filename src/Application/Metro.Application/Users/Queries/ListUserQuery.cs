using Metro.Application.Users.ViewModels;
using Metro.Shared.Queries;
using Metro.Shared.Results;

namespace Metro.Application.Users.Queries
{
    public class ListUserQuery : IQuery<ApiResult<ListUserResponse>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
    }
}
