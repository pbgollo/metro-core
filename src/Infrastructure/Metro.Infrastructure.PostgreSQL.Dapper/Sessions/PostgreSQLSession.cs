using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Metro.Infrastructure.PostgreSQL.Dapper.Sessions
{
    public class PostgreSQLSession : IDisposable
    {
        public IDbConnection Connection { get; }

        public PostgreSQLSession(IConfiguration configuration)
        {
            Connection = new NpgsqlConnection(configuration["PostgreSQL:Connection"]);
            Connection.Open();
        }

        public void Dispose() => Connection?.Dispose();
    }
}
