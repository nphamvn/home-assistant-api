using HomeAssistant.API.Models;
using HomeAssistant.API.Services.Interfaces;

namespace HomeAssistant.API.Entities;

public class Message : IHasId
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string SenderId { get; set; }
    public AppUser Sender { get; set; }
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; }
}