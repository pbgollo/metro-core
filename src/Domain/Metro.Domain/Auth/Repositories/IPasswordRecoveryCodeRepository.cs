using Metro.Domain.Auth.Entities;

namespace Metro.Domain.Auth.Repositories
{
    public interface IPasswordRecoveryCodeRepository
    {
        Task Create(PasswordRecoveryCode code);
        Task<PasswordRecoveryCode?> GetActiveByUserId(Guid userId);
        Task InvalidateAllActiveByUserId(Guid userId);
        Task Update(PasswordRecoveryCode code);
    }
}
