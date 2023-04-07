using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SimpleLiveChat.Services
{
    [Authorize]
    class MessageHub : BaseHub
    {
        public async Task Send(string chatId, string message)
        {
            var username = GetUsername();

            await this.Clients.Group(chatId).SendAsync($"message", chatId, username, message);
        }

        public async Task SendPrivate(string chatId, string toUser, string message)
        {
            var username = GetUsername();

            await this.Clients.User(toUser).SendAsync($"message-private", chatId, username, message);
        }
    }
}