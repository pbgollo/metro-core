namespace Metro.Domain.Users.Repositories
{
    public interface IPasswordRecoveryCodeRepository
    {
        Task Create(Entities.PasswordRecoveryCode code);
        Task<Entities.PasswordRecoveryCode?> GetActiveByUserId(Guid userId);
        Task InvalidateAllActiveByUserId(Guid userId);
        Task Update(Entities.PasswordRecoveryCode code);
    }
}
