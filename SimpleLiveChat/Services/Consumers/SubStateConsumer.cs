using System.Collections.Concurrent;
using System.Text.Json;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Consumers.Base;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Consumers
{
    public class SubStateConsumer : BaseEventConsumer<IServerEvent>, IConsumingState
    {
        private readonly IServiceProvider _serviceProvider;
        private ConcurrentBag<bool> _isListening = new ConcurrentBag<bool>();
        
        public SubStateConsumer(ISubscriberProvider provider, ILogger<SubStateConsumer> logger, IServiceProvider serviceProvider) : base(provider, logger)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Subscribe()
        {
            base.Subscribe();
            _isListening.Add(true);
        }

        public override void Dispose()
        {
            base.Dispose();
            _isListening.Clear();
        }

        public override string Channel => "SubState";

        public override Action<RedisChannel, RedisValue> ConsumeEvent => (channel, value) =>
            {
                var @event = JsonSerializer.Deserialize<ServerEvent>(value);

                if (@event.Node != _subscriber.Multiplexer.ClientName)
                {
                    Consume(channel, @event).GetAwaiter().GetResult();
                }
            };

        public override Task Consume(string channel, IServerEvent @event)
        {
            _logger.LogInformation($"Msg consumed on {Channel} channel", @event);

            using (var scope = _serviceProvider.CreateScope())
            {
                var consumer = scope.ServiceProvider.GetRequiredService<EventConsumer>();

                if (@event.EventType == EventType.ShutDown)
                {
                    consumer.Dispose();
                }

                if (@event.EventType == EventType.StartUp)
                {
                    consumer.Subscribe();
                }
            }
            return Task.CompletedTask;
        }

        public bool ConsumingEventState() =>  _isListening.Count > 0;
    }
}