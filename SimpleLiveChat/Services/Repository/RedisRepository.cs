using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SimpleLiveChat.Interfaces;
using SimpleLiveChat.Interfaces.Repository;
using SimpleLiveChat.Models.CustomExceptions;
using SimpleLiveChat.Services.Configuration;
using StackExchange.Redis;

namespace SimpleLiveChat.Services.Repository
{
    public class RedisRepository<T> : IStringKeyRepository<T>
    {
        private readonly IDatabase _cache;

        public RedisRepository(IDatabaseProvider provider)
        {
            _cache = provider.Database;
        }

        public async Task<T> Add(string key, T obj, TimeSpan? timeout = null)
        {
            var value = JsonSerializer.Serialize<T>(obj);
            await _cache.StringSetAsync(key, value, timeout);

            return obj;
        }

        public async Task Refresh(string key, TimeSpan timeout)
        {
            await _cache.KeyExpireAsync(key, timeout);
        }

        public async Task<T> Get(string key)
        {
            var temp = await _cache.StringGetAsync(key);

            if (!temp.HasValue) {
                throw new KeyNotFoundException<string,T>(key);
            }

            return JsonSerializer.Deserialize<T>(temp);
        }

        public async Task<T> Remove(string key)
        {
            var temp = await Get(key);
            await _cache.KeyDeleteAsync(key);

            return temp;
        }
    }
}