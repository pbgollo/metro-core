using Metro.Infrastructure.PostgreSQL.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Metro.Domain.Users.Repositories;
using Metro.Domain.Auth.Repositories;
using Metro.Infrastructure.PostgreSQL.Repositories.User;
using Metro.Infrastructure.PostgreSQL.Repositories.Auth;
using Metro.Shared.Data;
using Metro.Infrastructure.PostgreSQL.Data;

namespace Metro.Infrastructure.PostgreSQL.Extensions
{
    public static class PostgreSQLExtensions
    {
        public static IServiceCollection AddPostgreSQL(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddNpgsql<PostgreSQLContext>(configuration["PostgreSQL:Connection"]);
            services.AddScoped<IUnityOfWork, UnityOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPasswordRecoveryCodeRepository, PasswordRecoveryCodeRepository>();
            return services;
        }
    }
}
