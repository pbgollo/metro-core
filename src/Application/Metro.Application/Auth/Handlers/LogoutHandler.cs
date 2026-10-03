using Metro.Application.Auth.Commands;
using Metro.Domain.Auth.Services;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using Metro.Application.Messaging;
using Metro.Application.Results;

namespace Metro.Application.Auth.Handlers
{
    public class LogoutHandler : IHandler<LogoutCommand, object>
    {
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnityOfWork _unityOfWork;

        public LogoutHandler(
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IUnityOfWork unityOfWork)
        {
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _unityOfWork = unityOfWork;
        }

        public async Task<ApiResult<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var success = ApiResult<object>.Ok(message: "Logout realizado com sucesso.");

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return success;
            }

            var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
            var existingToken = await _refreshTokenRepository.GetByTokenHash(tokenHash);

            if (existingToken is null || !existingToken.IsActive)
            {
                return success;
            }

            await _unityOfWork.BeginAsync(cancellationToken);
            try
            {
                existingToken.Revoke();
                await _refreshTokenRepository.Update(existingToken);
                await _unityOfWork.CommitAsync(cancellationToken);
            }
            catch
            {
                await _unityOfWork.RollbackAsync(cancellationToken);
                throw;
            }

            return success;
        }
    }
}
