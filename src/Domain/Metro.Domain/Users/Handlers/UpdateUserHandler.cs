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

            await _unityOfWork.BeginAsync(cancellationToken);
            try
            {
                await _userRepository.Update(entity);
                await _unityOfWork.CommitAsync(cancellationToken);
            }
            catch
            {
                await _unityOfWork.RollbackAsync(cancellationToken);
                throw;
            }

            return CommandResult.OK();
        }
    }
}
