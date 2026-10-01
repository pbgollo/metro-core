using MediatR;
using Metro.Domain.Users.Commands;
using Metro.Domain.Users.Repositories;
using Metro.Shared.Data;
using Metro.Shared.Handlers;
using Metro.Shared.Results;

namespace Metro.Domain.Users.Handlers
{
    public class DeleteUserHandler : IHandler<DeleteUserCommand>
    {
        private readonly IUnityOfWork _unityOfWork;
        private readonly IUserRepository _userRepository;

        public DeleteUserHandler(IUnityOfWork unityOfWork, IUserRepository userRepository)
        {
            _unityOfWork = unityOfWork;
            _userRepository = userRepository;
        }

        public async Task<ICommandResult<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var entity = await _userRepository.GetById(request.Id);

            if (entity == null)
            {
                return CommandResult.NotFound();
            }

            await _userRepository.Delete(entity);
            await _unityOfWork.Commit();

            return CommandResult.OK();
        }
    }
}
