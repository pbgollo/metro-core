using Metro.Domain.Users.Authentication.Services;
using Microsoft.Extensions.Configuration;

namespace Metro.Infrastructure.Auth.Services
{
    public class PasswordRecoverySettings : IPasswordRecoverySettings
    {
        public int CodeLength { get; }
        public int CodeExpiresInMinutes { get; }
        public int MaxAttempts { get; }

        public PasswordRecoverySettings(IConfiguration configuration)
        {
            CodeLength = int.Parse(configuration["PasswordRecovery:CodeLength"] ?? "6");
            CodeExpiresInMinutes = int.Parse(configuration["PasswordRecovery:CodeExpiresInMinutes"] ?? "15");
            MaxAttempts = int.Parse(configuration["PasswordRecovery:MaxAttempts"] ?? "5");
        }
    }
}
