using Metro.Domain.Users.Authentication.Queries;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Authentication.ViewModel;
using Metro.Domain.Users.Repositories;
using Metro.Shared.QueryHandlers;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Authentication.Handlers
{
    public class LoginHandler : IQueryHandler<LoginQuery, ApiResult<LoginViewModel>>
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

        public async Task<ApiResult<LoginViewModel>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var unauthorized = ApiResult<LoginViewModel>.Unauthorized();
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
                return ApiResult<LoginViewModel>.Ok(new LoginViewModel
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
