using Metro.Domain.Users.Authentication;
using Metro.Domain.Users.Authentication.Services;
using Metro.Domain.Users.Commands;
using Metro.Domain.Users.Repositories;
using Metro.Domain.Users.ViewModel;
using Metro.Shared.Data;
using Metro.Shared.Handlers;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Handlers
{
    public class CreateUserHandler : IHandler<CreateUserCommand, CreatedId>
    {
        private readonly IUnityOfWork _unityOfWork;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public CreateUserHandler(
            IUnityOfWork unityOfWork,
            IUserRepository userRepository,
            IUserQueryRepository userQueryRepository,
            IPasswordService passwordService)
        {
            _unityOfWork = unityOfWork;
            _userRepository = userRepository;
            _userQueryRepository = userQueryRepository;
            _passwordService = passwordService;
        }

        public async Task<ApiResult<CreatedId>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            GetUserViewModel? userViewModel = await _userQueryRepository.GetByEmail(request.Email);

            if (userViewModel is not null)
            {
                return ApiResult<CreatedId>.Conflict("O e-mail já está cadastrado.");
            }

            if (!PasswordPolicy.IsValid(request.Password))
            {
                return ApiResult<CreatedId>.BadRequest(PasswordPolicy.RequirementsMessage);
            }

            var hashedPassword = _passwordService.HashPasswordWithSalt(request.Password);
            var role = request.Role is "master" or "client" ? request.Role : "client";

            var entity = new Entities.User(
                name: request.Name,
                email: request.Email,
                document: request.Document,
                phone: request.Phone,
                password: Convert.ToBase64String(hashedPassword),
                role: role,
                isActive: true
            );

            await _unityOfWork.BeginAsync(cancellationToken);
            try
            {
                await _userRepository.Create(entity);
                await _unityOfWork.CommitAsync(cancellationToken);
                return ApiResult<CreatedId>.Created(new CreatedId(entity.Id));
            }
            catch
            {
                await _unityOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
