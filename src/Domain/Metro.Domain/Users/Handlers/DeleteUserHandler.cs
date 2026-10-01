using Metro.Domain.Users.Commands;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using Metro.Shared.Handlers;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Handlers
{
    public class DeleteUserHandler : IHandler<DeleteUserCommand, object?>
    {
        private readonly IUnityOfWork _unityOfWork;
        private readonly IUserRepository _userRepository;

        public DeleteUserHandler(IUnityOfWork unityOfWork, IUserRepository userRepository)
        {
            _unityOfWork = unityOfWork;
            _userRepository = userRepository;
        }

        public async Task<ApiResult<object?>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var entity = await _userRepository.GetById(request.Id);

            if (entity == null)
            {
                return ApiResult<object?>.NotFound();
            }

            await _unityOfWork.BeginAsync(cancellationToken);
            try
            {
                await _userRepository.Delete(entity);
                await _unityOfWork.CommitAsync(cancellationToken);
                return ApiResult<object?>.Ok(message: "Request processed successfully.");
            }
            catch
            {
                await _unityOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
