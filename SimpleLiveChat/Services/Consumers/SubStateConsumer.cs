using System.Text.Json;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Consumers.Base;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Consumers
{
    public class SubStateConsumer : BaseEventConsumer<IServerEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        public SubStateConsumer(ISubscriberProvider provider, ILogger<SubStateConsumer> logger, IServiceProvider serviceProvider) : base(provider, logger)
        {
            _serviceProvider = serviceProvider;
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
                var consumer = scope.ServiceProvider.GetService<EventConsumer>();

                if (@event.EventType == EventType.ShutDown)
                {
                    consumer?.Dispose();
                }

                if (@event.EventType == EventType.StartUp)
                {
                    consumer?.Subscribe();
                }
            }
            return Task.CompletedTask;
        }
    }
}