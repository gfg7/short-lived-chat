using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveChat.Interfaces.Hubs;
using SimpleLiveChat.Interfaces.PublisherSubscriber;
using SimpleLiveChat.Interfaces.Repository;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;

namespace SimpleLiveChat.Services.Hubs
{
    [Authorize]
    class MessageHub : BaseHub<IMessageHub>
    {
        private readonly IHubContext<NotifyHub, INotifyHub> _notify;
        private readonly ITempStore<string> _tempStore;

        public MessageHub(IHubContext<NotifyHub, INotifyHub> notify, ITempStore<string> tempStore)
        {
            _notify = notify;
            _tempStore = tempStore;
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
            await _tempStore.Refresh(chatId, TimeSpan.FromMinutes(15));
        }

        public async Task SendPrivate(string chatId, string toUser, string message)
        {
            var username = GetUsername();

            await this.Clients.OthersInGroup(chatId).SendPrivate(chatId, null, $"Someone whispers...");

            var @event = new Event().SetChat(chatId)
                .SetPayload("Someone whispers...")
                .SetType(EventType.PrivateMessage);

            await _notify.Clients.All.Notify(@event);

            await this.Clients.User(toUser).SendPrivate(chatId, username, message);
        }
    }
}