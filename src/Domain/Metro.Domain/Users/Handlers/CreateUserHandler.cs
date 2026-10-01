using System.Net;
using MediatR;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Repositories;
using Metro.Domain.Users.Commands;
using Metro.Domain.Users.ViewModel;
using Metro.Shared.Data;
using Metro.Shared.Handlers;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Handlers
{
    public class CreateUserHandler : IHandler<CreateUserCommand>
    {
        private readonly IUnityOfWork _unityOfWork;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public CreateUserHandler(IUnityOfWork unityOfWork, IUserRepository userRepository, IUserQueryRepository userQueryRepository, IPasswordService passwordService)
        {
            _unityOfWork = unityOfWork;
            _userRepository = userRepository;
            _userQueryRepository = userQueryRepository;
            _passwordService = passwordService;
        }

        public async Task<ICommandResult<Unit>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            GetUserViewModel? userViewModel = await _userQueryRepository.GetByEmail(request.Email);

            if (userViewModel is not null)
            {
                return new CommandResult(statusCode: HttpStatusCode.Conflict, message: "O e-mail já está cadastrado.");
            }

            var hashedPassword = _passwordService.HashPasswordWithSalt(request.Password);
            var role = request.Role is "master" or "client" ? request.Role : "client";

            var entity = new Entities.User(
                name: request.Name,
                email: request.Email,
                document: request.Document,
                phone: request.Phone,
                password: Convert.ToBase64String(hashedPassword),
                role: role
            );
            await _unityOfWork.BeginAsync(cancellationToken);
            try
            {
                await _userRepository.Create(entity);
                await _unityOfWork.CommitAsync(cancellationToken);
                return CommandResult.Created(entity.Id);
            }
            catch
            {
                await _unityOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
