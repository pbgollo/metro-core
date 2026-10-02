using Metro.Application.Users.Queries;
using Metro.Application.Users.Repositories;
using Metro.Application.Users.ViewModels;
using Metro.Shared.QueryHandlers;
using Metro.Shared.Results;

namespace Metro.Application.Users.QueryHandlers
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
