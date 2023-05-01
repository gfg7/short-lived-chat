using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveChat.Interfaces.Hubs;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Interfaces.Repository;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Repository;

namespace SimpleLiveChat.Services.Hubs
{
    [Authorize]
    class MessageHub : BaseHub<IMessageHub>
    {
        private readonly IHubContext<NotifyHub, INotifyHub> _notify;
        private readonly ChatActivityService _chats;

        public MessageHub(IHubContext<NotifyHub, INotifyHub> notify, ChatActivityService chats)
        {
            _chats = chats;
            _notify = notify;
        }

        public async Task Send(string chatId, string message)
        {
            var username = GetUsername();

            var @event = new Event().SetChat(chatId)
                .SetInvoker(username)
                .SetPayload(message)
                .SetType(EventType.Message);

            await _notify.Clients.All.Notify(@event);

            await this.Clients.Group(chatId).Send(chatId, username, message);
            await _chats.SentMessage(chatId, username);
        }

        public async Task SendPrivate(string chatId, string toUser, string message)
        {
            var username = GetUsername();

            await this.Clients.OthersInGroup(chatId).SendPrivate(chatId, null, $"Someone whispers...");

            var @event = new Event().SetChat(chatId)
                .SetPayload("Someone whispers...")
                .SetType(EventType.PrivateMessage);

            await _notify.Clients.All.Notify(@event);
            await _chats.SentMessage(chatId, username, true);

            await this.Clients.User(toUser).SendPrivate(chatId, username, message);
        }
    }
}