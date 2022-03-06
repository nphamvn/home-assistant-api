using HomeAssistant.API.Models;

namespace HomeAssistant.API.Entities;

public class ConversationName
{
    public string? Name { get; set; }
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }
}