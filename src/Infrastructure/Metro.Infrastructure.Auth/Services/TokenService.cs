using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public string GenerateToken(Domain.Users.Entities.User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var jwtKey = _configuration["Authentication:JWT:Key"]
                ?? throw new InvalidOperationException("Authentication:JWT:Key is not configured.");
            var key = Encoding.ASCII.GetBytes(jwtKey);

            int expiresInDays = int.Parse(_configuration["Authentication:JWT:ExpiresInDays"] ?? "7");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(expiresInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return handler.WriteToken(token);
        }
    }
}
