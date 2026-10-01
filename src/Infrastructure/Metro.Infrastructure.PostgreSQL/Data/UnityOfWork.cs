using Metro.Infrastructure.PostgreSQL.Contexts;
using Metro.Shared.Data;

namespace Metro.Infrastructure.PostgreSQL.Data
{
    public class UnityOfWork : IUnityOfWork
    {
        private readonly PostgreSQLContext _context;
        public UnityOfWork(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task<int> Commit()
            => await _context.SaveChangesAsync();

        public void Rollback()
            => _context.Database.CurrentTransaction?.Rollback();
    }
}
