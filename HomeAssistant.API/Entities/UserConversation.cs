using HomeAssistant.API.Models;

namespace HomeAssistant.API.Entities;

public class UserConversation
{
    public string? Name { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; }
}