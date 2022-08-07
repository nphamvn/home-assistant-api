using System.Text;
using System.Text.Json;
using HomeAssistant.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace HomeAssistant.API.Services.Chat;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task Set<T>(string key, T value)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));
        var setString = JsonSerializer.Serialize(value);
        //_memoryCache.Set<string>(key, setString, cacheEntryOptions);
        var bytes = Encoding.UTF8.GetBytes(setString);
        await _cache.SetAsync(key, bytes);
        //return Task.CompletedTask;
    }

    public async Task<T> Get<T>(string key)
    {
        var ret = default(T);
        var bytes = await _cache.GetAsync(key);
        if (bytes != null)
        {
            //return Task.FromResult<T>(default(T));
            var stringValue = Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrEmpty(stringValue))
            {
                ret = JsonSerializer.Deserialize<T>(stringValue);
            }
        }
        return ret;
    }

    public async Task Remove(string key)
    {
        //_memoryCache.Remove(key);
        await _cache.RemoveAsync(key);
    }
}