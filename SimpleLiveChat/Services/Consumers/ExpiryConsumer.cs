using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.Hubs;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Consumers.Base;
using SimpleLiveChat.Services.Hubs;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Consumers
{
    public class ExpiryConsumer : HubEventConsumer
    {
        private readonly IHubContext<NotifyHub, INotifyHub> _notify;
        private readonly IHubContext<ChatHub, IChatHub> _chat;
        public ExpiryConsumer(ISubscriberProvider provider,
                              ILogger<ExpiryConsumer> logger,
                              HubContextWrapper hubContextWrapper)
                              : base(provider, logger, hubContextWrapper)
        {
            _chat = hubContextWrapper.Get<ChatHub, IChatHub>();
            _notify = hubContextWrapper.Get<NotifyHub, INotifyHub>();
        }
        public override string Channel => "__key*__:*";

        public override Action<RedisChannel, RedisValue> ConsumeEvent => SetUpCallback();

        public override Task Consume(string channel, IExternalHubEvent @event)
        {
            _logger.LogInformation($"Msg consumed on {Channel} channel", @event);

            _chat.Clients.Group(@event.ChatId).Leave(@event);
            _notify.Clients.Group(@event.ChatId).Notify(@event);

            return Task.CompletedTask;
        }

        private Action<RedisChannel, RedisValue> SetUpCallback()
        {
            return (key, value) =>
            {
                _logger.LogInformation($"Event emerged on {Channel} channel", key, value);

                var type = KeyService.GetKeyPart(key.ToString());

                if (!Enum.IsDefined(typeof(CacheKeyType), type))
                {
                    throw new NotSupportedException($"Consumed {type} cache key type is not defined");
                }

                var id = KeyService.GetKeyPart(key.ToString(), 1);
                var @event = new Event()
                .SetTime(DateTime.UtcNow)
                .SetChat(id)
                .SetType(EventType.SystemNotif);

                if (type is nameof(CacheKeyType.Chat))
                {
                    @event = @event
                    .SetPayload("Chat has ended because of inactivity :(");
                }

                if (type is nameof(CacheKeyType.Activity))
                {
                    var username = KeyService.GetKeyPart(key.ToString(), 2);

                    @event = @event
                    .SetPayload($"User {username} is kicked out because of inactivity :(");
                }

                Consume(Channel, @event).GetAwaiter().GetResult();
            };
        }
    }
}