using Metro.Application.Auth.Commands;
using Metro.Domain.Auth.Services;
using Metro.Shared.Data;
using Metro.Application.Messaging;
using Metro.Application.Results;
using Metro.Domain.Auth.Entities;
using Metro.Domain.Auth.Repositories;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Repositories;

namespace Metro.Application.Auth.Handlers
{
    public class ResetPasswordHandler : IHandler<ResetPasswordCommand, object>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordRecoveryCodeRepository _passwordRecoveryCodeRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnityOfWork _unityOfWork;

        public ResetPasswordHandler(
            IUserRepository userRepository,
            IPasswordRecoveryCodeRepository passwordRecoveryCodeRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordService passwordService,
            IUnityOfWork unityOfWork)
        {
            _userRepository = userRepository;
            _passwordRecoveryCodeRepository = passwordRecoveryCodeRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordService = passwordService;
            _unityOfWork = unityOfWork;
        }

        public async Task<ApiResult<object>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var invalid = ApiResult<object>.Unauthorized("Código inválido ou expirado.");

            var user = await _userRepository.GetEmail(request.Email!.Trim());
            if (user is null || !user.IsActive)
            {
                return invalid;
            }

            var recovery = await _passwordRecoveryCodeRepository.GetActiveByUserId(user.Id);
            if (recovery is null || !recovery.IsActive)
            {
                return invalid;
            }

            var codeHash = _passwordService.HashCode(request.Code!.Trim());
            if (!string.Equals(recovery.CodeHash, codeHash, StringComparison.Ordinal))
            {
                await _unityOfWork.BeginAsync(cancellationToken);
                try
                {
                    recovery.RegisterFailedAttempt();
                    await _passwordRecoveryCodeRepository.Update(recovery);
                    await _unityOfWork.CommitAsync(cancellationToken);
                }
                catch
                {
                    await _unityOfWork.RollbackAsync(cancellationToken);
                    throw;
                }

                return invalid;
            }

            var hashedPassword = _passwordService.HashPassword(request.NewPassword!);

            await _unityOfWork.BeginAsync(cancellationToken);
            try
            {
                user.UpdatePassword(hashedPassword);
                await _userRepository.Update(user);

                recovery.MarkUsed();
                await _passwordRecoveryCodeRepository.Update(recovery);

                await _refreshTokenRepository.RevokeAllActiveByUserId(user.Id);
                await _unityOfWork.CommitAsync(cancellationToken);
            }
            catch
            {
                await _unityOfWork.RollbackAsync(cancellationToken);
                throw;
            }

            return ApiResult<object>.Ok(message: "Senha atualizada com sucesso.");
        }
    }
}
