using Microsoft.EntityFrameworkCore;
using Metro.Domain.Users.Entities;
using Metro.Infrastructure.PostgreSQL.Contexts;

namespace Metro.Infrastructure.PostgreSQL.Initializers
{
    public static class PostgreSQLInitializer
    {
        public static void MigrateDatabase(this PostgreSQLContext context)
        {
            context.Database.Migrate();
        }

        public static void SeedMasterUser(this PostgreSQLContext context, string hashedPassword)
        {
            if (context.Users.Any(u => u.Email == "master"))
            {
                return;
            }

            var master = new User(
                name: "Master",
                email: "master",
                document: "00000000000",
                phone: "00000000000",
                password: hashedPassword,
                role: "master",
                isActive: true
            );

            context.Users.Add(master);
            context.SaveChanges();
        }
    }
}
