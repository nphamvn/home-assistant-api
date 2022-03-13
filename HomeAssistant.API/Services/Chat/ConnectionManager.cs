using HomeAssistant.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HomeAssistant.API.Services.Chat;

public class ConnectionManager
{
    private readonly ICacheService _cacheService;

    public ConnectionManager(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Add(string username, string connectionId)
    {
        var connections = await Get(username);
        if (connections == null)
        {
            connections = new List<string>();
        }
        connections.Add(connectionId);
        await _cacheService.Set<List<string>>(username, connections);
    }

    public async Task<List<string>> Get(string username)
    {
        return await _cacheService.Get<List<string>>(username);
    }

    public void Remove(string username, string connectionId)
    {
        var connections = Get(username).Result;
        if (connections.Contains(connectionId))
        {
            connections.Remove(connectionId);
        }
        _cacheService.Set(username, connections);
    }
}