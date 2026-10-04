using Metro.Shared.Entities;

namespace Metro.Domain.Auth.Entities
{
    public class PasswordRecoveryCode : Entity
    {
        public Guid UserId { get; private set; }

        public string CodeHash { get; private set; }

        public DateTime ExpiresAt { get; private set; }

        public DateTime? UsedAt { get; private set; }

        public int AttemptCount { get; private set; }

        public int MaxAttempts { get; private set; }

        public bool IsActive => UsedAt is null && ExpiresAt > DateTime.UtcNow && AttemptCount < MaxAttempts;

        public PasswordRecoveryCode(Guid userId, string codeHash, DateTime expiresAt, int maxAttempts)
        {
            UserId = userId;
            CodeHash = codeHash;
            ExpiresAt = expiresAt;
            MaxAttempts = maxAttempts;
        }

        public void RegisterFailedAttempt() => AttemptCount++;

        public void MarkUsed() => UsedAt = DateTime.UtcNow;

        public void Invalidate()
        {
            if (UsedAt is null)
            {
                UsedAt = DateTime.UtcNow;
            }
        }
    }
}
