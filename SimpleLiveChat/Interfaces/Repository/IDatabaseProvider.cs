using StackExchange.Redis;

namespace SimpleLiveChat.Interfaces.Repository
{
    public interface IDatabaseProvider
    {
        IDatabase Database {get;}
    }
}