namespace Metro.Domain.Users.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task Create(Entities.RefreshToken refreshToken);
        Task<Entities.RefreshToken?> GetByTokenHash(string tokenHash);
        Task RevokeAllActiveByUserId(Guid userId);
        Task Update(Entities.RefreshToken refreshToken);
    }
}
