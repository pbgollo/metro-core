namespace Metro.Shared.Data
{
    public interface IUnityOfWork
    {
        Task<int> Commit();
        void Rollback();
    }
}
