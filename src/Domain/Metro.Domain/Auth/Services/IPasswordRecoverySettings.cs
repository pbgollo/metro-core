namespace Metro.Domain.Auth.Services
{
    public interface IPasswordRecoverySettings
    {
        int CodeLength { get; }
        int CodeExpiresInMinutes { get; }
        int MaxAttempts { get; }
    }
}
