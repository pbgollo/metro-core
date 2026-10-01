using Metro.Infrastructure.PostgreSQL.Contexts;
using Metro.Shared.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Metro.Infrastructure.PostgreSQL.Data
{
    public class UnityOfWork : IUnityOfWork, IAsyncDisposable
    {
        private readonly PostgreSQLContext _context;
        private IDbContextTransaction? _transaction;

        public UnityOfWork(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task BeginAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is not null)
                return;

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            _transaction ??= await _context.Database.BeginTransactionAsync(cancellationToken);
            var transaction = _transaction;

            try
            {
                var result = await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                _context.ChangeTracker.Clear();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            _context.ChangeTracker.Clear();
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
