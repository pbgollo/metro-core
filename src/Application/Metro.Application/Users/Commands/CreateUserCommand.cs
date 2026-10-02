using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Application.Users.Commands
{
    public class CreateUserCommand : ICommand<ApiResult<CreatedId>>
    {
        required public string Name { get; set; }

        required public string Email { get; set; }

        required public string Document { get; set; }

        required public string Phone { get; set; }

        required public string Password { get; set; }

        public string Role { get; set; } = "client";
    }
}
