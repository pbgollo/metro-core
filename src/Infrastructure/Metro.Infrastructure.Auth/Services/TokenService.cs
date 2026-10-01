using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Metro.Domain.Users.Authentication.Services;

namespace Metro.Infrastructure.Auth.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(Domain.Users.Entities.User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var jwtKey = _configuration["Authentication:JWT:Key"]
                ?? throw new InvalidOperationException("Authentication:JWT:Key is not configured.");
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var expiresInMinutes = GetAccessTokenExpiresInMinutes();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                ]),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return handler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public string HashRefreshToken(string refreshToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToHexString(hash);
        }

        public int GetAccessTokenExpiresInSeconds()
            => GetAccessTokenExpiresInMinutes() * 60;

        public DateTime GetRefreshTokenExpiresAt()
        {
            var expiresInDays = int.Parse(_configuration["Authentication:JWT:RefreshTokenExpiresInDays"] ?? "7");
            return DateTime.UtcNow.AddDays(expiresInDays);
        }

        private int GetAccessTokenExpiresInMinutes()
            => int.Parse(_configuration["Authentication:JWT:AccessTokenExpiresInMinutes"] ?? "30");
    }
}
