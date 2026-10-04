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
    public class VerifyPasswordRecoveryCodeHandler : IHandler<VerifyPasswordRecoveryCodeCommand, object>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordRecoveryCodeRepository _passwordRecoveryCodeRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnityOfWork _unityOfWork;

        public VerifyPasswordRecoveryCodeHandler(
            IUserRepository userRepository,
            IPasswordRecoveryCodeRepository passwordRecoveryCodeRepository,
            IPasswordService passwordService,
            IUnityOfWork unityOfWork)
        {
            _userRepository = userRepository;
            _passwordRecoveryCodeRepository = passwordRecoveryCodeRepository;
            _passwordService = passwordService;
            _unityOfWork = unityOfWork;
        }

        public async Task<ApiResult<object>> Handle(VerifyPasswordRecoveryCodeCommand request, CancellationToken cancellationToken)
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

            return ApiResult<object>.Ok(message: "Código válido.");
        }
    }
}
