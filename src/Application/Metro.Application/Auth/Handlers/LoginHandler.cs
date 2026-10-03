using Metro.Application.Auth.Commands;
using Metro.Domain.Auth.Services;
using Metro.Application.Auth.ViewModels;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using Metro.Application.Messaging;
using Metro.Application.Results;

namespace Metro.Application.Auth.Handlers
{
    public class LoginHandler : IHandler<LoginCommand, LoginViewModel>
    {
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnityOfWork _unityOfWork;

        public LoginHandler(
            ITokenService tokenService,
            IPasswordService passwordService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnityOfWork unityOfWork)
        {
            _tokenService = tokenService;
            _passwordService = passwordService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unityOfWork = unityOfWork;
        }

        public async Task<ApiResult<LoginViewModel>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var unauthorized = ApiResult<LoginViewModel>.Unauthorized();

            var user = await _userRepository.GetEmail(request.Email!);

            if (user is null)
            {
                return unauthorized;
            }

            if (!_passwordService.ConfirmPassword(user.Password, request.Password!) || !user.IsActive)
            {
                return unauthorized;
            }

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken(
                user.Id,
                _tokenService.HashRefreshToken(refreshToken),
                _tokenService.GetRefreshTokenExpiresAt());

            await _unityOfWork.BeginAsync(cancellationToken);
            try
            {
                await _refreshTokenRepository.RevokeAllActiveByUserId(user.Id);
                await _refreshTokenRepository.Create(refreshTokenEntity);
                await _unityOfWork.CommitAsync(cancellationToken);
            }
            catch
            {
                await _unityOfWork.RollbackAsync(cancellationToken);
                throw;
            }

            return ApiResult<LoginViewModel>.Ok(new LoginViewModel
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken,
                ExpiresIn = _tokenService.GetAccessTokenExpiresInSeconds()
            });
        }
    }
}
