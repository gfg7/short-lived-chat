using System.Text.Json;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Publisher
{
    class EventPublisher<T> : IPublisher<T> where T: class, IServerEvent
    {
        private readonly ILogger<EventPublisher<T>> _logger;
        private readonly ISubscriber _subscriber;
        private readonly string _channel = Environment.GetEnvironmentVariable("DEFAULT_TOPIC");
        public EventPublisher(ISubscriberProvider provider, ILogger<EventPublisher<T>> logger)
        {
            _logger = logger;
            _subscriber = provider.Subscriber;
        }

        public async Task Publish(T internalEvent, string channel=null)
        {
            internalEvent.SetNode(_subscriber.Multiplexer.ClientName);

            var payload = JsonSerializer.Serialize(internalEvent);
            await _subscriber.PublishAsync(new RedisChannel(channel ?? _channel, RedisChannel.PatternMode.Auto), new RedisValue(payload));
            _logger.LogInformation($"Msg sent to {channel} channel", internalEvent);
        }
    }
}