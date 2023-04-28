using System.Text.Json;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.PublisherSubscrib;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Consumers.Base
{
    public abstract class BaseEventConsumer<T> : IConsumer<T> where T : ILocalEvent
    {
        public abstract string Channel { get; }
        protected readonly ISubscriber _subscriber;
        protected readonly ILogger<BaseEventConsumer<T>> _logger;
        public BaseEventConsumer(ISubscriberProvider provider, ILogger<BaseEventConsumer<T>> logger)
        {
            _subscriber = provider.Subscriber;
            _logger = logger;
        }

        public virtual void Subscribe()
        {
            _logger.LogInformation($"Start up consumer {this.GetType().Name} listening {Channel} channel");
            
            _subscriber.Subscribe(
               new RedisChannel(Channel, RedisChannel.PatternMode.Auto),
               ConsumeEvent
           );
        }

        public abstract Action<RedisChannel, RedisValue> ConsumeEvent {get;}
        public abstract Task Consume(string channel, T @event);

        public virtual void Dispose()
        {
            _logger.LogInformation($"Shut down consumer {this.GetType().Name} listening {Channel} channel");
            _subscriber.Unsubscribe(Channel, ConsumeEvent);
        }
    }
}