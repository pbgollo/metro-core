using Metro.Application.Users.Queries;
using Metro.Application.Users.Repositories;
using Metro.Application.Users.ViewModels;
using Metro.Shared.QueryHandlers;
using Metro.Shared.Results;

namespace Metro.Application.Users.QueryHandlers
{
    public class ListUserHandler : IQueryHandler<ListUserQuery, ApiResult<ListUserResponse>>
    {
        private readonly IUserQueryRepository _userQueryRepository;

        public ListUserHandler(IUserQueryRepository userQueryRepository)
        {
            _userQueryRepository = userQueryRepository;
        }

        public async Task<ApiResult<ListUserResponse>> Handle(ListUserQuery query, CancellationToken cancellationToken)
        {
            var list = await _userQueryRepository.List(query.Page, query.PageSize, query.Search);
            var totalCount = await _userQueryRepository.Count(query.Search);
            return ApiResult<ListUserResponse>.Ok(new ListUserResponse { Items = list, TotalCount = totalCount });
        }
    }
}
