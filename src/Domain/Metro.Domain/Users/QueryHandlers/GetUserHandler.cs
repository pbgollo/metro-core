using Metro.Domain.Users.Queries;
using Metro.Domain.Users.Repositories;
using Metro.Domain.Users.ViewModel;
using Metro.Shared.QueryHandlers;
using Metro.Shared.Results;

namespace Metro.Domain.Users.QueryHandlers
{
    public class GetUserHandler : IQueryHandler<GetUserQuery, ApiResult<GetUserViewModel>>
    {
        private readonly IUserQueryRepository _userQueryRepository;

        public GetUserHandler(IUserQueryRepository userQueryRepository)
        {
            _userQueryRepository = userQueryRepository;
        }

        public async Task<ApiResult<GetUserViewModel>> Handle(GetUserQuery query, CancellationToken cancellationToken)
        {
            var vm = await _userQueryRepository.GetById(query.Id);
            if (vm is null)
            {
                return ApiResult<GetUserViewModel>.NotFound();
            }

            return ApiResult<GetUserViewModel>.Ok(vm);
        }
    }
}
