namespace Metro.Domain.Users.Authentication.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(Entities.User user);
        string GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);
        int GetAccessTokenExpiresInSeconds();
        DateTime GetRefreshTokenExpiresAt();
    }
}
