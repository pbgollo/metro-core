namespace Metro.Domain.Users.Repositories
{
    public interface IUserRepository
    {
        Task<Entities.User?> GetById(Guid id);
        Task Create(Entities.User user);
        Task Update(Entities.User user);
        Task Delete(Entities.User user);
        Task<Entities.User?> GetEmail(string email);
    }
}
