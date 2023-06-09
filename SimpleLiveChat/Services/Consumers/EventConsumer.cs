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

namespace SimpleLiveChat.Services.Consumers
{
    public class EventConsumer : HubEventConsumer, IDelayed
    {
        public override string Channel => Environment.GetEnvironmentVariable("DEFAULT_TOPIC");
        public EventConsumer(ISubscriberProvider provider, IServiceProvider serviceProvider, ILogger<EventConsumer> logger)
         : base(provider, serviceProvider, logger)
        {
        }

        public override async Task Consume(string channel, IExternalHubEvent @event)
        {
            _logger.LogInformation($"Msg consumed on {Channel} channel", @event);

            var t = base.Consume(channel, @event);
            if (!t.IsCompletedSuccessfully)
            {
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                if (Enum.GetName(typeof(EventType), @event.EventType) is not null)
                {
                    var consumer = scope.ServiceProvider.GetRequiredService<IHubContext<NotifyHub, INotifyHub>>() as INotifyHub;
                    await consumer.Notify(@event);
                    return;
                }

                throw new NotSupportedException();
            }
        }

        public override void SetUpCallback()
        {
            ConsumeEvent = async (channel, value) =>
            {
                var @event = JsonSerializer.Deserialize<Event>(value);
                await Consume(channel, @event);
            };
        }
    }
}