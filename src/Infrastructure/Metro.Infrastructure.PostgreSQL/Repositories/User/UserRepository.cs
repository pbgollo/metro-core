using Microsoft.EntityFrameworkCore;
using Metro.Domain.Users.Repositories;
using Metro.Infrastructure.PostgreSQL.Contexts;

namespace Metro.Infrastructure.PostgreSQL.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly PostgreSQLContext _context;

        public UserRepository(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task Create(Domain.Users.Entities.User user)
            => await _context.Users.AddAsync(user);

        public async Task<Domain.Users.Entities.User?> GetById(Guid id)
            => await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

        public async Task Update(Domain.Users.Entities.User user)
            => _context.Entry(user).State = EntityState.Modified;

        public async Task Delete(Domain.Users.Entities.User user)
        {
            _context.Users.Remove(user);
        }
        public async Task<Domain.Users.Entities.User?> GetEmail(string email)
            => await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

    }
}
