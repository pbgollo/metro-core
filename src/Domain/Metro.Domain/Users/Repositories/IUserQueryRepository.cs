using Metro.Domain.Users.ViewModel;

namespace Metro.Domain.Users.Repositories
{
    public interface IUserQueryRepository
    {
        Task<GetUserViewModel?> GetById(Guid id);
        Task<IEnumerable<ListUserViewModel?>> List(int page, int pageSize, string? search = null);
        Task<int> Count(string? search = null);
        Task<GetUserViewModel?> GetByEmail(string email);
    }
}
