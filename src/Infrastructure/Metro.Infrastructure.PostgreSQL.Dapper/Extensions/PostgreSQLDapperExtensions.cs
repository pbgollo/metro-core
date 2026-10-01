using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Metro.Infrastructure.PostgreSQL.Dapper.Sessions;
using Metro.Infrastructure.PostgreSQL.Dapper.Repositories;
using Metro.Domain.Users.Repositories;

namespace Metro.Infrastructure.PostgreSQL.Dapper.Extensions
{
    public static class PostgreSQLDapperExtensions
    {
        public static IServiceCollection AddPostgreSQLDapper(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<PostgreSQLSession>();
            services.AddScoped<IUserQueryRepository, UserQueryRepository>();
            return services;
        }
    }
}
