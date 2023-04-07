using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SimpleLiveChat.Services
{
    class Repository<T> where T : new()
    {
        private readonly IDistributedCache _cache;

        public Repository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> Add(string key, T obj, TimeSpan? timeout = null)
        {
            await _cache.SetAsync(key, await ToByte(obj), new DistributedCacheEntryOptions()
            {
                SlidingExpiration = timeout
            });

            return obj;
        }

        public async Task Refresh(string key)
        {
            await _cache.RefreshAsync(key);
        }

        public async Task<IEnumerable<T>> Get(string username)
        {
            var temp = await _cache.GetAsync(username, new CancellationToken());
            var obj = await FromByte(temp);

            return obj;
        }

        private async Task<byte[]> ToByte(T obj)
        {
            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync<T>(stream, obj);
                return stream.ToArray();
            }
        }

        private async Task<IEnumerable<T>> FromByte(byte[] obj)
        {
            using (var stream = new MemoryStream(obj))
            {
                var temp = JsonSerializer.DeserializeAsyncEnumerable<T>(stream).ToBlockingEnumerable();
                return temp;
            }
        }
    }
}