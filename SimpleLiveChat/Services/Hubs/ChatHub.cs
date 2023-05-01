using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.Hubs;
using SimpleLiveChat.Interfaces.Repository;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;
using SimpleLiveChat.Services.Repository;

namespace SimpleLiveChat.Services.Hubs
{
    [Authorize]
    public class ChatHub : BaseHub<IChatHub>
    {
        private readonly ChatActivityService _chats;
        private readonly IHubContext<NotifyHub, INotifyHub> _notify;
        public ChatHub(ChatActivityService chats, IHubContext<NotifyHub, INotifyHub> notify)
        {
            _chats = chats;
            _notify = notify;
        }

        public async Task CreateChat(string chat)
        {
            var newChat = new Chat(chat);
            await _chats.CreateChat(newChat, GetUsername());

            var connId = this.Context.ConnectionId;
            await this.Groups.AddToGroupAsync(connId, newChat.Id.ToString());

            var @event = new Event()
                .SetChat(newChat.Id.ToString())
                .SetPayload(newChat)
                .SetType(EventType.NewChat) as IHubEvent;

            await this.Clients.Caller.CreateChat(@event);
            await _notify.Clients.All.Notify(@event);
        }

        public async Task JoinIn(string chatId)
        {
            var connId = this.Context.ConnectionId;
            var username = GetUsername();

            await this.Groups.AddToGroupAsync(connId, chatId);
            await _chats.JoinInChat(chatId, username, DateTime.UtcNow);

            var @event = new Event()
                .SetChat(chatId)
                .SetPayload($"{username} has joined the chat!")
                .SetType(EventType.SystemNotif) as IHubEvent;

            await this.Clients.OthersInGroup(chatId).JoinIn(@event);
            await _notify.Clients.Group(chatId).Notify(@event);
        }

        public async Task Leave(string chatId)
        {
            var username = GetUsername();
            var connId = this.Context.ConnectionId;

            await this.Groups.RemoveFromGroupAsync(connId, chatId);
            await _chats.LeaveChat(chatId, username);

            var @event = new Event()
                .SetChat(chatId)
                .SetPayload($"{username} bids farewell...")
                .SetType(EventType.SystemNotif) as IHubEvent;

            await this.Clients.OthersInGroup(chatId).Leave(@event);
            await _notify.Clients.Group(chatId).Notify(@event);
        }
    }
}