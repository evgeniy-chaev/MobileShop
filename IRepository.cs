namespace MobileShop
{
    public interface IRepository<T> : IDisposable
         where T : class
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
        IAsyncEnumerable<MobilePhone> GetNameFilteredAsync(string filter, CancellationToken cancellationToken);
        void Create(T item);
        void Update(T item);
        void Delete(int id);
    }
}
