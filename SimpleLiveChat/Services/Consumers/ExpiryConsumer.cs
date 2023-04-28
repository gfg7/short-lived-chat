using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.Hubs;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Consumers.Base;
using SimpleLiveChat.Services.Hubs;
using SimpleLiveChat.Services.Hubs.Chats;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Consumers
{
    public class ExpiryConsumer : BaseEventConsumer<IHubEvent>
    {
        private readonly IHubContext<NotifyHub, INotifyHub> _notify;
        private readonly IHubContext<ChatHub, IChatHub> _chat;

        public ExpiryConsumer(ISubscriberProvider provider,
                              ILogger<ExpiryConsumer> logger,
                              IHubContext<NotifyHub, INotifyHub> notify,
                              IHubContext<ChatHub, IChatHub> chat) : base(provider, logger)
        {
            _chat = chat;
            _notify = notify;
        }

        public override string Channel => "__key*__:*";

        public override Action<RedisChannel, RedisValue> ConsumeEvent => SetUpCallback();

        public override Task Consume(string channel, IHubEvent @event)
        {
            _logger.LogInformation($"Msg consumed on {Channel} channel", @event);

            _chat.Clients.Group(@event.ChatId).Leave(@event);
            _notify.Clients.Group(@event.ChatId).Notify(@event);

            return Task.CompletedTask;
        }

        private Action<RedisChannel, RedisValue> SetUpCallback()
        {
            return async (key, value) =>
            {
                _logger.LogInformation($"Event emerged on {Channel} channel", key, value);

                var id = KeyExtractor.GetKey(key.ToString());
                var type = KeyExtractor.GetEntity(key.ToString());

                var @event = new Event()
                                .SetChat(id)
                                .SetTime(DateTime.UtcNow) as IHubEvent;

                if (type is nameof(Chat) && value.ToString() is "expired")
                {
                    @event.SetPayload("Chat has ended because of inactivity :(");
                    @event.SetType(EventType.SystemNotif);

                    await Consume(Channel, @event);
                    return;
                }

                throw new NotSupportedException();
            };
        }
    }
}