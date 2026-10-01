namespace Metro.Domain.Users.Authentication.Services
{
    public interface IPasswordRecoverySettings
    {
        int CodeLength { get; }
        int CodeExpiresInMinutes { get; }
        int MaxAttempts { get; }
    }
}
