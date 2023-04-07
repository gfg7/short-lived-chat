using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SimpleLiveChat.Models;

namespace SimpleLiveChat.Services
{
    [Authorize]
    class ChatHub : BaseHub
    {
        private readonly Repository<Chat> _chats;
        public ChatHub(Repository<Chat> chats)
        {
            _chats = chats;
        }

        private void SetUserChats(IEnumerable<string> chats)
        {
            this.Context.GetHttpContext().Response.Cookies.Delete("chats");
            this.Context.GetHttpContext().Response.Cookies.Append("chats", chats.ToString());
        }

        private IEnumerable<string> GetUserChats()
        {
            return this.Context.GetHttpContext().Request.Cookies["chats"]?.Cast<string>() ?? new List<string>();
        }

        public async Task<IEnumerable<Chat>> GetUserChat() {
            var username = GetUsername();
            var chats = await _chats.Get(username);

            return chats;
        }

        public async Task CreateChat(string chat)
        {
            var newChat = new Chat(chat);
            await _chats.Add(newChat.Id.ToString(), newChat);
            var connId = this.Context.ConnectionId;
            await this.Groups.AddToGroupAsync(connId, newChat.Id.ToString());

            var userChats = GetUserChats().Append(newChat.Id.ToString());
            SetUserChats(userChats);

            await this.Clients.Caller.SendAsync($"new-chat", newChat);
            await this.Clients.All.SendAsync($"notify", newChat);
        }

        public async Task JoinIn(string chatId)
        {
            if (GetUserChats().Contains(chatId))
            {
                return;
            }

            var connId = this.Context.ConnectionId;
            var username = GetUsername();

            await this.Groups.AddToGroupAsync(connId, chatId);
            var userChats = GetUserChats().Append(chatId);
            SetUserChats(userChats);

            await this.Clients.OthersInGroup(chatId).SendAsync("system-msg", chatId, $"{username} has joined the chat!");
        }

        public async Task Leave(string chatId)
        {
            var username = GetUsername();
            var connId = this.Context.ConnectionId;

            await this.Groups.RemoveFromGroupAsync(connId, chatId);
            var userChats = GetUserChats().TakeWhile(x=> x!=chatId);
            SetUserChats(userChats);

            await this.Clients.OthersInGroup(chatId).SendAsync("system-msg", chatId, $"{username} bids farewell...");
        }
    }
}