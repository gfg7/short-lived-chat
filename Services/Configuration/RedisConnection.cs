using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Interfaces.Repository;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Configuration
{
    public class RedisConnection : ISubscriberProvider, IDatabaseProvider
    {
        private readonly IConnectionMultiplexer _connection;
        public RedisConnection()
        {
            var settings = new ConfigurationOptions() {
                 ClientName = Guid.NewGuid().ToString()    
            };

            settings.EndPoints.Add(Environment.GetEnvironmentVariable("REDIS_HOST"));

            _connection = ConnectionMultiplexer.Connect(settings);

            // _connection = ConnectionMultiplexer.SentinelConnect(settings);
        }

        public IDatabase Database => _connection.GetDatabase();

        public ISubscriber Subscriber => _connection.GetSubscriber();
    }
}