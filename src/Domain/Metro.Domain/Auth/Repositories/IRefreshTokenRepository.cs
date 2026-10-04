using Metro.Domain.Auth.Entities;

namespace Metro.Domain.Auth.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task Create(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenHash(string tokenHash);
        Task RevokeAllActiveByUserId(Guid userId);
        Task Update(RefreshToken refreshToken);
    }
}
