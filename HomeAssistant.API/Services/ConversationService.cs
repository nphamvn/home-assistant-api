using HomeAssistant.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.API.Services.Chat;

public class ConversationService
{
    private readonly ApplicationDbContext _context;

    public ConversationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task GetUserConversation(string username)
    {
        // var conversations = await _context.Conversations
        //             .Where(c => c.Creator.UserName == username || c.Partner.UserName == username)
        //             .Include(c => c.Creator)
        //             .Include(c => c.Partner)
        //             .ToListAsync();
        //AsQueryable
    }
}