using Dapper;
using Metro.Application.Users.Repositories;
using Metro.Application.Users.ViewModels;
using Metro.Infrastructure.PostgreSQL.Dapper.Sessions;

namespace Metro.Infrastructure.PostgreSQL.Dapper.Repositories
{
    public class UserQueryRepository : IUserQueryRepository
    {
        private readonly PostgreSQLSession _session;
        public UserQueryRepository(PostgreSQLSession session)
        {
            _session = session;
        }

        public async Task<GetUserViewModel?> GetById(Guid id)
        {
            var sql = @"
                SELECT
                    U.""Id"",
                    U.""Name"",
                    U.""Email"",
                    U.""Document"",
                    U.""Phone"",
                    U.""Role"",
                    U.""IsActive""
                FROM
                    ""User"" AS U
                WHERE
                    U.""Id"" = @Id
            ";
            var get = await _session.Connection.QueryAsync<GetUserViewModel>(sql, new
            {
                Id = id
            });
            return get.SingleOrDefault();
        }

        public async Task<IEnumerable<ListUserViewModel?>> List(int page, int pageSize, string? search = null)
        {
            var sql = @"
                SELECT
                    U.""Id"",
                    U.""Name"",
                    U.""Email"",
                    U.""Document"",
                    U.""Phone"",
                    U.""Role"",
                    U.""IsActive""
                FROM
                    ""User"" AS U
                WHERE
                    (@SearchPattern IS NULL OR U.""Name"" ILIKE @SearchPattern OR U.""Email"" ILIKE @SearchPattern)
                ORDER BY
                    U.""Name"" ASC
                LIMIT
                    @PageSize
                OFFSET
                    @OffSet;
            ";

            var searchPattern = string.IsNullOrWhiteSpace(search) ? null : $"%{search.Trim()}%";

            return await _session.Connection.QueryAsync<ListUserViewModel>(sql, new
            {
                PageSize = pageSize,
                OffSet = (page - 1) * pageSize,
                SearchPattern = searchPattern
            });
        }

        public async Task<int> Count(string? search = null)
        {
            var searchPattern = string.IsNullOrWhiteSpace(search) ? null : $"%{search.Trim()}%";

            return await _session.Connection.ExecuteScalarAsync<int>(@"
                SELECT COUNT(*)
                FROM ""User"" AS U
                WHERE (@SearchPattern IS NULL OR U.""Name"" ILIKE @SearchPattern OR U.""Email"" ILIKE @SearchPattern);
            ", new
            {
                SearchPattern = searchPattern
            });
        }

        public async Task<GetUserViewModel?> GetByEmail(string email)
        {
            var sql = @"
                SELECT
                    U.""Id"",
                    U.""Name"",
                    U.""Email"",
                    U.""Document"",
                    U.""Phone"",
                    U.""Role"",
                    U.""IsActive""
                FROM
                    ""User"" AS U
                WHERE
                    U.""Email"" = @Email
                LIMIT 1;
            ";
            var get = await _session.Connection.QueryAsync<GetUserViewModel>(sql, new
            {
                Email = email
            });
            return get.SingleOrDefault();
        }

    }
}
