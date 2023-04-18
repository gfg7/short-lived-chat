using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SimpleLiveChat.Interfaces;
using SimpleLiveChat.Interfaces.Repository;
using SimpleLiveChat.Models.CustomExceptions;
using SimpleLiveChat.Services.Configuration;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Repository
{
    public class RedisRepository<T> : IStringKeyRepository<T>, ITempStore<string> where T : IEntity
    {
        private readonly IDatabase _cache;

        public RedisRepository(IDatabaseProvider provider)
        {
            _cache = provider.Database;
        }

        private string GetFullKey(string key) => string.Join(":", key, typeof(T).Name);

        public async Task<T> Add(string key, T obj, TimeSpan? timeout = null)
        {
            var value = JsonSerializer.Serialize<T>(obj);
            key = GetFullKey(key);
            await _cache.StringSetAsync(key, value, timeout);

            return obj;
        }

        public async Task Refresh(string key, TimeSpan timeout)
        {
            key = GetFullKey(key);

            await _cache.KeyExpireAsync(key, timeout);
        }

        public async Task<T> Get(string key)
        {
            key = GetFullKey(key);

            var temp = await _cache.StringGetAsync(key);

            if (!temp.HasValue) {
                throw new KeyNotFoundException<string,T>(key);
            }

            return JsonSerializer.Deserialize<T>(temp);
        }

        public async Task<T> Remove(string key)
        {
            key = GetFullKey(key);

            var temp = await Get(key);
            await _cache.KeyDeleteAsync(key);

            return temp;
        }
    }
}