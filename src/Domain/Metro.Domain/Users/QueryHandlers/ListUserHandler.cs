using Metro.Domain.Users.Repositories;
using Metro.Domain.Users.Queries;
using Metro.Domain.Users.ViewModel;
using Metro.Shared.QueryHandlers;
using Metro.Shared.Returns;

namespace Metro.Domain.Users.QueryHandlers
{
    public class ListUserHandler : IQueryHandler<ListUserQuery, Return<ListUserResponse>>
    {
        private readonly IUserQueryRepository _userQueryRepository;

        public ListUserHandler(IUserQueryRepository userQueryRepository)
        {
            _userQueryRepository = userQueryRepository;
        }

        public async Task<Return<ListUserResponse>> Handle(ListUserQuery query, CancellationToken cancellationToken)
        {
            var list = await _userQueryRepository.List(query.Page, query.PageSize, query.Search);
            var totalCount = await _userQueryRepository.Count(query.Search);
            return Return<ListUserResponse>.OK(new ListUserResponse { Items = list, TotalCount = totalCount });
        }
    }
}
