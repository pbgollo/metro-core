using Microsoft.EntityFrameworkCore;
using Metro.Domain.Auth.Entities;
using Metro.Domain.Auth.Repositories;
using Metro.Infrastructure.PostgreSQL.Contexts;

namespace Metro.Infrastructure.PostgreSQL.Repositories.Auth
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly PostgreSQLContext _context;

        public RefreshTokenRepository(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task Create(RefreshToken refreshToken)
            => await _context.RefreshTokens.AddAsync(refreshToken);

        public async Task<RefreshToken?> GetByTokenHash(string tokenHash)
            => await _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

        public async Task RevokeAllActiveByUserId(Guid userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoke();
            }
        }

        public Task Update(RefreshToken refreshToken)
        {
            _context.Entry(refreshToken).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}
