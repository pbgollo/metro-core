using Metro.Domain.Users.ViewModel;
using Metro.Shared.Queries;
using Metro.Shared.Returns;

namespace Metro.Domain.Users.Queries
{
    public class GetUserQuery: IQuery<Return<GetUserViewModel>>
    {
        public Guid Id { get; set; }
    }
}
