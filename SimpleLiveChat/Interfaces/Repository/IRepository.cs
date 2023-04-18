namespace SimpleLiveChat.Interfaces.Repository
{
    public interface IRepository<T, K> where T : notnull
    {
        Task<K> Add(T key, K obj, TimeSpan? timeout = null);
        Task<K> Get(T key);
        Task<K> Remove(T key);
    }

    public interface IStringKeyRepository<K> : IRepository<string, K> { }
}