using HomeAssistant.API.Data;
using HomeAssistant.API.Entities;

namespace HomeAssistant.API.Services;

public class MessageRepository : Repository<Message>
{
    public MessageRepository(ApplicationDbContext dbContext) : base(dbContext, dbContext.Messages)
    {
    }
}