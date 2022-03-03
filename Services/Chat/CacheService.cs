using System.Text.Json;
using HomeAssistant.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace HomeAssistant.API.Services.Chat;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public Task Set<T>(string key, T value)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));
        var setString = JsonSerializer.Serialize(value);
        _memoryCache.Set<string>(key, setString, cacheEntryOptions);
        return Task.CompletedTask;
    }

    public Task<T> Get<T>(string key)
    {
        var stringValue = _memoryCache.Get<string>(key);
        if (stringValue == null)
        {
            return Task.FromResult<T>(default(T));
        }
        var ret = JsonSerializer.Deserialize<T>(stringValue);
        return Task.FromResult(ret);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}