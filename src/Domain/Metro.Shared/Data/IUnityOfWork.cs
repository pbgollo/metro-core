namespace Metro.Shared.Data
{
    public interface IUnityOfWork
    {
        Task BeginAsync(CancellationToken cancellationToken = default);
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
