using Metro.Domain.Users.ViewModel;
using Metro.Shared.Queries;
using Metro.Shared.Returns;

namespace Metro.Domain.Users.Queries
{
    public class ListUserQuery : IQuery<Return<ListUserResponse>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
    }
}
