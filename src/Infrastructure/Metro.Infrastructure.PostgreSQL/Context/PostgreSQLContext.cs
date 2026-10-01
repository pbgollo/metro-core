using Microsoft.EntityFrameworkCore;
using Metro.Domain.Users.Entities;
using Metro.Infrastructure.PostgreSQL.Mappings.User;
using Metro.Shared.Entities;

namespace Metro.Infrastructure.PostgreSQL.Contexts
{
    public class PostgreSQLContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserMap());
        }

        public PostgreSQLContext(DbContextOptions<PostgreSQLContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            var addedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Added).ToList();

            addedEntities.ForEach(E =>
            {
                if (typeof(Entity).IsInstanceOfType(E.Entity) && (
                    E.Property("CreatedAt").CurrentValue == null
                    || ((DateTime)E.Property("CreatedAt").CurrentValue).CompareTo(DateTime.MinValue) == 0
                ))
                {
                    E.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            });

            var editedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();

            editedEntities.ForEach(E =>
            {
                if (typeof(Entity).IsInstanceOfType(E.Entity) && (
                    E.Property("UpdatedAt").CurrentValue == null
                    || ((DateTime)E.Property("UpdatedAt").CurrentValue).CompareTo(DateTime.MinValue) == 0
                    || ((DateTime)E.Property("UpdatedAt").CurrentValue) < DateTime.UtcNow
                ))
                {
                    E.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
