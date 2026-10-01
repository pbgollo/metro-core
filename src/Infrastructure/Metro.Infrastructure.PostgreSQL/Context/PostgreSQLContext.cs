using Microsoft.EntityFrameworkCore;
using Metro.Domain.Users.Entities;
using Metro.Infrastructure.PostgreSQL.Mappings.User;
using Metro.Shared.Entities;

namespace Metro.Infrastructure.PostgreSQL.Contexts
{
    public class PostgreSQLContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordRecoveryCode> PasswordRecoveryCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserMap());
            builder.ApplyConfiguration(new RefreshTokenMap());
            builder.ApplyConfiguration(new PasswordRecoveryCodeMap());
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
                if (!typeof(Entity).IsInstanceOfType(E.Entity))
                    return;

                var createdAt = E.Property("CreatedAt").CurrentValue as DateTime?;
                if (createdAt is null || createdAt == DateTime.MinValue)
                {
                    E.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            });

            var editedEntities = ChangeTracker.Entries().Where(E => E.State == EntityState.Modified).ToList();

            editedEntities.ForEach(E =>
            {
                if (!typeof(Entity).IsInstanceOfType(E.Entity))
                    return;

                var updatedAt = E.Property("UpdatedAt").CurrentValue as DateTime?;
                if (updatedAt is null || updatedAt == DateTime.MinValue || updatedAt < DateTime.UtcNow)
                {
                    E.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            });

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
