namespace SimpleLiveChat.Interfaces.Repository
{
    public interface ITempStore<T> where T : notnull
    {
        Task Refresh(T key, TimeSpan timeout);
    }
}