using HomeAssistant.API.Data;
using HomeAssistant.API.Entities;
using HomeAssistant.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.API.Services;

public class ConversationRepository : Repository<Conversation>
{
    public ConversationRepository(ApplicationDbContext dbContext) : base(dbContext, dbContext.Conversations)
    {
    }
}