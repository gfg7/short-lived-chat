using System.Security.Claims;
using SimpleLiveChat.Models;

namespace SimpleLiveChat.Services.Hubs.Chats
{
    public partial class ChatHub {
        protected IEnumerable<string> GetChats()
        {
            return this.Context.User.Claims
            .Where(x => x.Type == nameof(Chat))
            .Select(x => x.Value) ?? new List<string>();
        }

        protected void AddToChat(string chatId)
        {
            this.Context.User?.Claims.Append(new(nameof(Chat), chatId));
        }

        protected void RemoveFromChat(string chatId)
        {
            var user = this.Context.User.Identity as ClaimsIdentity;
            user.TryRemoveClaim(new(nameof(Chat), chatId));
        }
    }
}