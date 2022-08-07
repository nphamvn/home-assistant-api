using HomeAssistant.API.Data;
using HomeAssistant.API.Entities;
using HomeAssistant.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.API.Services.Chat;

public class MessageService
{
    private readonly ICacheService _cacheService;

    public MessageService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task AddToPendingMessage(string username, Message message)
    {
        var pendingMessages = await _cacheService.Get<List<Message>>(username);
        if (pendingMessages == null)
        {
            pendingMessages = new List<Message>();
        }
        pendingMessages.Add(message);
        await _cacheService.Set(username, pendingMessages);
    }

    /// <summary>
    /// Get messages that are not delivered yet (when the user is offline)
    /// </summary>
    /// <param name="conversationId"></param>
    /// <returns></returns>
    public async Task<List<Message>> GetPendingMessage(string username)
    {
        return new List<Message>();
        var pendingMessages = await _cacheService.Get<List<Message>>(username);
        if (pendingMessages == null)
        {
            return new List<Message>();
        }
        return pendingMessages;
    }
}