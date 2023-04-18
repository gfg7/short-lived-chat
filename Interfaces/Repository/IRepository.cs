namespace SimpleLiveChat.Interfaces.Repository
{
    public interface IRepository<T, K> where T : class where K: notnull
    {
        Task<K> Add(string key, K obj, TimeSpan? timeout = null);
        Task<K> Get(string key);
        Task<K> Remove(string key);
    }

    public interface IStringKeyRepository<T> : IRepository<string, T> where T: notnull { }
}