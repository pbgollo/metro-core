using Metro.Shared.Entities;

namespace Metro.Domain.Auth.Entities
{
    public class RefreshToken : Entity
    {
        public Guid UserId { get; private set; }

        public string TokenHash { get; private set; }

        public DateTime ExpiresAt { get; private set; }

        public DateTime? RevokedAt { get; private set; }

        public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

        public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt)
        {
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = expiresAt;
        }

        public void Revoke() => RevokedAt = DateTime.UtcNow;
    }
}
