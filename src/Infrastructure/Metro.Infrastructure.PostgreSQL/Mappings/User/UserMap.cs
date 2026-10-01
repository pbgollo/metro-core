using Microsoft.EntityFrameworkCore;

namespace Metro.Infrastructure.PostgreSQL.Mappings.User
{
    public class UserMap : IEntityTypeConfiguration<Metro.Domain.Users.Entities.User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Metro.Domain.Users.Entities.User> builder)
        {
            builder.ToTable("User");
            builder.Property(x => x.Id);
            builder.Property(x => x.Name);
            builder.Property(x => x.Email);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.Document);
            builder.Property(x => x.Phone);
            builder.Property(x => x.Password);
            builder.Property(x => x.Role);
            builder.Property(x => x.IsActive);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
        }
    }
}
