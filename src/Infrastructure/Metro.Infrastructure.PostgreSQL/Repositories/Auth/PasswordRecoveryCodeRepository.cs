using Microsoft.EntityFrameworkCore;
using Metro.Domain.Auth.Entities;
using Metro.Domain.Auth.Repositories;
using Metro.Infrastructure.PostgreSQL.Contexts;

namespace Metro.Infrastructure.PostgreSQL.Repositories.Auth
{
    public class PasswordRecoveryCodeRepository : IPasswordRecoveryCodeRepository
    {
        private readonly PostgreSQLContext _context;

        public PasswordRecoveryCodeRepository(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task Create(PasswordRecoveryCode code)
            => await _context.PasswordRecoveryCodes.AddAsync(code);

        public async Task<PasswordRecoveryCode?> GetActiveByUserId(Guid userId)
            => await _context.PasswordRecoveryCodes
                .Where(x => x.UserId == userId
                    && x.UsedAt == null
                    && x.ExpiresAt > DateTime.UtcNow
                    && x.AttemptCount < x.MaxAttempts)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

        public async Task InvalidateAllActiveByUserId(Guid userId)
        {
            var codes = await _context.PasswordRecoveryCodes
                .Where(x => x.UserId == userId
                    && x.UsedAt == null
                    && x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var code in codes)
            {
                code.Invalidate();
            }
        }

        public Task Update(PasswordRecoveryCode code)
        {
            _context.Entry(code).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}
