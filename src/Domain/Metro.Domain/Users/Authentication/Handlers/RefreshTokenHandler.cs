using Metro.Domain.Users.Authentication.Commands;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Authentication.ViewModel;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using Metro.Shared.Handlers;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Authentication.Handlers
{
    public class RefreshTokenHandler : IHandler<RefreshTokenCommand, LoginViewModel>
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnityOfWork _unityOfWork;

        public RefreshTokenHandler(
            ITokenService tokenService,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnityOfWork unityOfWork)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unityOfWork = unityOfWork;
        }

        public async Task<ApiResult<LoginViewModel>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var unauthorized = ApiResult<LoginViewModel>.Unauthorized();

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return unauthorized;
            }

            var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
            var existingToken = await _refreshTokenRepository.GetByTokenHash(tokenHash);

            if (existingToken is null || !existingToken.IsActive)
            {
                return unauthorized;
            }

            var user = await _userRepository.GetById(existingToken.UserId);
            if (user is null || !user.IsActive)
            {
                return unauthorized;
            }

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenEntity = new RefreshToken(
                user.Id,
                _tokenService.HashRefreshToken(newRefreshToken),
                _tokenService.GetRefreshTokenExpiresAt());

            await _unityOfWork.BeginAsync(cancellationToken);
            try
            {
                existingToken.Revoke();
                await _refreshTokenRepository.Update(existingToken);
                await _refreshTokenRepository.Create(newRefreshTokenEntity);
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
                RefreshToken = newRefreshToken,
                ExpiresIn = _tokenService.GetAccessTokenExpiresInSeconds()
            });
        }
    }
}
