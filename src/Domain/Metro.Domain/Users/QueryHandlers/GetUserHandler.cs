using Metro.Domain.Users.Repositories;
using Metro.Domain.Users.Queries;
using Metro.Domain.Users.ViewModel;
using Metro.Shared.QueryHandlers;
using Metro.Shared.Returns;

namespace Metro.Domain.Users.QueryHandlers
{
    public class GetUserHandler : IQueryHandler<GetUserQuery, Return<GetUserViewModel>>
    {
        private readonly IUserQueryRepository _userQueryRepository;

        public GetUserHandler(IUserQueryRepository userQueryRepository)
        {
            _userQueryRepository = userQueryRepository;
        }

        public async Task<Return<GetUserViewModel>> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            var vm = await _userQueryRepository.GetById(query.Id);
            if (vm is null)
            {
                return Return<GetUserViewModel>.NotFound(new GetUserViewModel());
            }

            return Return<GetUserViewModel>.OK(vm);
        }
    }
}
