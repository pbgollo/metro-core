using Metro.Shared.Commands;
using Metro.Shared.Results;

namespace Metro.Application.Users.Commands
{
    public class UpdateUserCommand : ICommand<ApiResult<object?>>
    {
        public Guid Id { get; set; }

        required public string Name { get; set; }

        required public string Email { get; set; }

        required public string Document { get; set; }

        required public string Phone { get; set; }

        public string? Password { get; set; }

        public string Role { get; set; } = "client";

        public bool? IsActive { get; set; }
    }
}
