namespace StajYerim.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Gerekirse özel repository'leri buraya property olarak ekleyeceğiz
        // IUserRepository Users { get; }

        Task<int> CompleteAsync();
    }
}
