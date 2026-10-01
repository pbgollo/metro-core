using Metro.Domain.Users.Authentication.ViewModel;
using Metro.Shared.Queries;
using Metro.Shared.Returns;

namespace Metro.Domain.Users.Authentication.Queries
{
    public class LoginQuery : IQuery<Return<LoginViewModel>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
