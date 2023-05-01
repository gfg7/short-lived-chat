using SimpleLiveChat.Interfaces.Repository;
using SimpleLiveChat.Models;
using SimpleLiveChat.Models.Entity;

namespace SimpleLiveChat.Services.Repository
{
    public class ChatActivityService
    {
        private readonly IStringKeyRepository<Chat> _chats;
        private readonly IStringKeyRepository<DateTime> _userChatActivity;
        private readonly IStringKeyRepository<List<Chat>> _userChatsGroups;

        public ChatActivityService(IStringKeyRepository<Chat> chats, IStringKeyRepository<DateTime> userChatActivity, IStringKeyRepository<List<Chat>> userChatsGroups)
        {
            _chats = chats;
            _userChatActivity = userChatActivity;
            _userChatsGroups = userChatsGroups;
        }

        public async Task CreateChat(Chat chat, string username)
        {
            var key = KeyService.CreateKey(nameof(CacheKeyType.Chat), chat.Id.ToString());
            await _chats.Add(key, chat, TimeSpan.FromMinutes(15));
            await JoinInChat(chat.Id.ToString(), username, DateTime.UtcNow);
        }

        public async Task<IEnumerable<Chat>> JoinInChat(string chatId, string username, DateTime joinedAt)
        {
            var userChats = await _userChatsGroups.Get(username);

            if (userChats.Any(x=> x.Id.ToString()==chatId))
            {
                return userChats;
            }

            var key = KeyService.CreateKey(nameof(CacheKeyType.User), username);
            var chat = await _chats.Get(chatId);
            userChats.Add(chat);
            await _userChatsGroups.Add(key, userChats);

            key = KeyService.CreateKey(nameof(CacheKeyType.Activity),chatId, username);
            await _userChatActivity.Add(key, joinedAt, TimeSpan.FromMinutes(15));

            return userChats;
        }

        public async Task<DateTime> LeaveChat(string chatId, string username)
        {
            var userChats = await GetUserChats(username);
            userChats.RemoveAll(x=> x.Id.ToString()==chatId);

            var key = KeyService.CreateKey(nameof(CacheKeyType.User), username);

            await _userChatsGroups.Add(key, userChats);

            key = KeyService.CreateKey(nameof(CacheKeyType.Activity),chatId, username);
            return await _userChatActivity.Remove(key);
        }

        public async Task SentMessage(string chatId, string username, bool isPrivate = false)
        {
            var key = KeyService.CreateKey(nameof(CacheKeyType.Activity),chatId, username);

            if (!isPrivate)
            {
                await _chats.Refresh(key, TimeSpan.FromMinutes(5));
            }

            await _userChatActivity.Refresh(key, TimeSpan.FromMinutes(15));
        }

        public async Task<List<Chat>> GetUserChats(string username)
        {
            var key = KeyService.CreateKey(nameof(CacheKeyType.User), username);

            return await _userChatsGroups.Get(key);
        }
    }
}