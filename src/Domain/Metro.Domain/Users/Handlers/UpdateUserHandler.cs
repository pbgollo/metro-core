using MediatR;
using Metro.Domain.Services;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Commands;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using Metro.Shared.Handlers;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Handlers
{
    public class UpdateUserHandler : IHandler<UpdateUserCommand>
    {
        private readonly IUnityOfWork _unityOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;

        public UpdateUserHandler(
            IUnityOfWork unityOfWork,
            IUserRepository userRepository,
            IEmailService emailService,
            IPasswordService passwordService)
        {
            _unityOfWork = unityOfWork;
            _userRepository = userRepository;
            _emailService = emailService;
            _passwordService = passwordService;
        }

        public async Task<ICommandResult<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var entity = await _userRepository.GetById(request.Id);
            if (entity == null)
            {
                return CommandResult.NotFound();
            }

            bool wasActive = entity.IsActive;
            var role = request.Role is "master" or "client" ? request.Role : entity.Role;

            entity.Update(
                name: request.Name,
                email: request.Email,
                document: request.Document,
                phone: request.Phone,
                role: role,
                isActive: request.IsActive
            );

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var hashedPassword = _passwordService.HashPasswordWithSalt(request.Password);
                entity.UpdatePassword(Convert.ToBase64String(hashedPassword));
            }

            await _userRepository.Update(entity);
            await _unityOfWork.Commit();

            if (!wasActive && entity.IsActive && !string.IsNullOrEmpty(entity.Email))
            {
                await _emailService.Send(entity.Email, "Ativação de Usuário", $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #1a1a1a; color: #f1f1f1; border-radius: 8px;'>
                    <h2 style='text-align: center; color: #FFD700;'>{entity.Name}, seu usuário foi liberado!</h2>
                    <p style='font-size: 16px; line-height: 1.5;'>Agora você pode acessar sua conta!</p>
                    <hr style='border: 1px solid #FFD700; margin: 15px 0;'>
                    <br>
                    <p style='text-align: center; font-size: 14px; color: #999;'>Este e-mail foi gerado automaticamente pelo sistema.</p>
                </div>");
            }

            return CommandResult.OK();
        }
    }
}
