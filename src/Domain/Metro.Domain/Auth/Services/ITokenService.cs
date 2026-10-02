using Metro.Domain.Users.Entities;

namespace Metro.Domain.Auth.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);
        int GetAccessTokenExpiresInSeconds();
        DateTime GetRefreshTokenExpiresAt();
    }
}
