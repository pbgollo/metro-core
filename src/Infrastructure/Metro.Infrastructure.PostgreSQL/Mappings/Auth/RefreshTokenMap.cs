using Microsoft.EntityFrameworkCore;
using Metro.Domain.Auth.Entities;

namespace Metro.Infrastructure.PostgreSQL.Mappings.Auth
{
    public class RefreshTokenMap : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken");
            builder.Property(x => x.Id);
            builder.Property(x => x.UserId);
            builder.Property(x => x.TokenHash);
            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.HasIndex(x => x.UserId);
            builder.Property(x => x.ExpiresAt);
            builder.Property(x => x.RevokedAt);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.Ignore(x => x.IsActive);
        }
    }
}
