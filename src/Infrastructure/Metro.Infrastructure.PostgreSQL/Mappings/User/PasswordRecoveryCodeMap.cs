using Microsoft.EntityFrameworkCore;
using Metro.Domain.Users.Entities;

namespace Metro.Infrastructure.PostgreSQL.Mappings.User
{
    public class PasswordRecoveryCodeMap : IEntityTypeConfiguration<PasswordRecoveryCode>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<PasswordRecoveryCode> builder)
        {
            builder.ToTable("PasswordRecoveryCode");
            builder.Property(x => x.Id);
            builder.Property(x => x.UserId);
            builder.HasIndex(x => x.UserId);
            builder.Property(x => x.CodeHash);
            builder.Property(x => x.ExpiresAt);
            builder.Property(x => x.UsedAt);
            builder.Property(x => x.AttemptCount);
            builder.Property(x => x.MaxAttempts);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.Ignore(x => x.IsActive);
        }
    }
}
