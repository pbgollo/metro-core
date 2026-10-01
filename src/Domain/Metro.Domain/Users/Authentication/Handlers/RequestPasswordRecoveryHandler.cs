using Metro.Domain.Users.Authentication.Commands;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Entities;
using Metro.Domain.Users.Repositories;
using Metro.Domain.Services;
using Metro.Shared.Data;
using Metro.Shared.Handlers;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Authentication.Handlers
{
    public class RequestPasswordRecoveryHandler : IHandler<RequestPasswordRecoveryCommand, object>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordRecoveryCodeRepository _passwordRecoveryCodeRepository;
        private readonly IPasswordService _passwordService;
        private readonly IPasswordRecoverySettings _settings;
        private readonly IEmailService _emailService;
        private readonly IUnityOfWork _unityOfWork;

        public RequestPasswordRecoveryHandler(
            IUserRepository userRepository,
            IPasswordRecoveryCodeRepository passwordRecoveryCodeRepository,
            IPasswordService passwordService,
            IPasswordRecoverySettings settings,
            IEmailService emailService,
            IUnityOfWork unityOfWork)
        {
            _userRepository = userRepository;
            _passwordRecoveryCodeRepository = passwordRecoveryCodeRepository;
            _passwordService = passwordService;
            _settings = settings;
            _emailService = emailService;
            _unityOfWork = unityOfWork;
        }

        public async Task<ApiResult<object>> Handle(RequestPasswordRecoveryCommand request, CancellationToken cancellationToken)
        {
            var success = ApiResult<object>.Ok(message: "Se o e-mail estiver cadastrado, você receberá um código de recuperação.");

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return success;
            }

            var user = await _userRepository.GetEmail(request.Email.Trim());
            if (user is null || !user.IsActive)
            {
                return success;
            }

            var code = _passwordService.GenerateNumericCode(_settings.CodeLength);
            var entity = new PasswordRecoveryCode(
                user.Id,
                _passwordService.HashCode(code),
                DateTime.UtcNow.AddMinutes(_settings.CodeExpiresInMinutes),
                _settings.MaxAttempts);

            await _unityOfWork.BeginAsync(cancellationToken);
            try
            {
                await _passwordRecoveryCodeRepository.InvalidateAllActiveByUserId(user.Id);
                await _passwordRecoveryCodeRepository.Create(entity);
                await _unityOfWork.CommitAsync(cancellationToken);
            }
            catch
            {
                await _unityOfWork.RollbackAsync(cancellationToken);
                throw;
            }

            await _emailService.Send(
                user.Email,
                "Recuperação de senha",
                $"<p>Seu código de recuperação é: <strong>{code}</strong></p><p>Ele é válido por {_settings.CodeExpiresInMinutes} minutos.</p>");

            return success;
        }
    }
}
