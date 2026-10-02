using Metro.Domain.Auth;
using Metro.Domain.Auth.Services;
using Metro.Application.Users.Commands;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using Metro.Shared.Handlers;
using Metro.Shared.Results;

namespace Metro.Application.Users.Handlers
{
    public class UpdateUserHandler : IHandler<UpdateUserCommand, object?>
    {
        private readonly IUnityOfWork _unityOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public UpdateUserHandler(
            IUnityOfWork unityOfWork,
            IUserRepository userRepository,
            IPasswordService passwordService)
        {
            _unityOfWork = unityOfWork;
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<ApiResult<object?>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var entity = await _userRepository.GetById(request.Id);
            if (entity == null)
            {
                return ApiResult<object?>.NotFound();
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
                if (!PasswordPolicy.IsValid(request.Password))
                {
                    return ApiResult<object?>.BadRequest(PasswordPolicy.RequirementsMessage);
                }

                var hashedPassword = _passwordService.HashPassword(request.Password);
                entity.UpdatePassword(hashedPassword);
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

            return ApiResult<object?>.Ok(message: "Request processed successfully.");
        }
    }
}
