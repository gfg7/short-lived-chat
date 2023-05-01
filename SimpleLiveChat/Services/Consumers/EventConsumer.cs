using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.Hubs;
using SimpleLiveChat.Interfaces.PublisherSubscrib;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Consumers.Base;
using SimpleLiveChat.Services.Hubs;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Consumers
{
    public class EventConsumer : HubEventConsumer, IDelayed
    {
        public override string Channel => Environment.GetEnvironmentVariable("DEFAULT_TOPIC") ?? nameof(SimpleLiveChat);

        public override Action<RedisChannel, RedisValue> ConsumeEvent => (channel, value) =>
            {
                var @event = JsonSerializer.Deserialize<Event>(value);
                Consume(channel, @event).GetAwaiter().GetResult();
            };

        public EventConsumer(ISubscriberProvider provider, ILogger<EventConsumer> logger, HubContextWrapper hubContextWrapper)
         : base(provider, logger, hubContextWrapper) { }

        public override async Task Consume(string channel, IExternalHubEvent @event)
        {
            _logger.LogInformation($"Msg consumed on {Channel} channel", @event);

            var t = base.Consume(channel, @event);
            if (t.IsCanceled)
            {
                return;
            }

            if (Enum.GetName(typeof(EventType), @event.EventType) is not null)
                {
                    var consumer = _hubContextWrapper.Get<NotifyHub, INotifyHub>();
                    await consumer.Clients.All.Notify(@event);
                    return;
                }

                throw new NotSupportedException($"Consumed {nameof(@event.EventType)} event type is not supported");
        }
    }
}