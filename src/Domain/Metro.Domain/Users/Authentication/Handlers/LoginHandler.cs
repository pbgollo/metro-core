using Metro.Domain.Users.Authentication.Queries;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Authentication.ViewModel;
using Metro.Domain.Users.Repositories;
using Metro.Shared.QueryHandlers;
using Metro.Shared.Returns;

namespace Metro.Domain.Users.Authentication.Handlers
{
    public class LoginHandler : IQueryHandler<LoginQuery, Return<LoginViewModel>>
    {
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;

        public LoginHandler(ITokenService tokenService, IPasswordService passwordService, IUserRepository userRepository)
        {
            _tokenService = tokenService;
            _passwordService = passwordService;
            _userRepository = userRepository;
        }

        public async Task<Return<LoginViewModel>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var unauthorized = Return<LoginViewModel>.Unauthorized(new LoginViewModel());
            var user = await _userRepository.GetEmail(request.Email);

            if (user is null)
            {
                return unauthorized;
            }

            try
            {
                var passwordBytes = Convert.FromBase64String(user.Password);
                if (!_passwordService.ConfirmPassword(passwordBytes, request.Password) || !user.IsActive)
                {
                    return unauthorized;
                }

                var token = _tokenService.GenerateToken(user);
                return Return<LoginViewModel>.OK(new LoginViewModel()
                {
                    Token = token
                });
            }
            catch (FormatException)
            {
                return unauthorized;
            }
        }
    }
}
