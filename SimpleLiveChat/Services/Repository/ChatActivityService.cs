using SimpleLiveChat.Interfaces.Entities;
using SimpleLiveChat.Interfaces.Hubs;
using SimpleLiveChat.Interfaces.Repository;
using SimpleLiveChat.Models;

namespace SimpleLiveChat.Services.Repository
{
    public class ChatActivityService
    {
        private readonly IStringKeyRepository<Chat> _chats;
        private readonly IStringKeyRepository<DateTime> _userChatsGroups;

        public async Task CreateChat(Chat chat, string username)
        {
            await _chats.Add(chat.Id.ToString(), chat, TimeSpan.FromMinutes(15));
            await JoinInChat(chat.Id.ToString(), username, DateTime.UtcNow);
        }

        public async Task JoinInChat(string chatId, string username, DateTime joinedAt)
        {
            var key = KeyService.CreateKey(chatId,username);
            await _userChatsGroups.Add(key, joinedAt, TimeSpan.FromMinutes(15));
        }

        public async Task<DateTime> LeaveChat(string chatId, string username)
        {
            var key = KeyService.CreateKey(chatId,username);
            return await _userChatsGroups.Remove(key);
        }

        public async Task SentMessage(string chatId, string username, bool isPrivate = false)
        {
            var key = KeyService.CreateKey(chatId,username);

            if (!isPrivate)
            {
                await _chats.Refresh(key, TimeSpan.FromMinutes(5));
            }

            await _userChatsGroups.Refresh(key, TimeSpan.FromMinutes(15));
        }
    }
}