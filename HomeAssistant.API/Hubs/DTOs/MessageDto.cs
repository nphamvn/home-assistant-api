namespace HomeAssistant.API.Hubs.DTOs;

public class MessageDto
{
    public int? ConversationId { get; set; }
    public string? ClientConversationId { get; set; }
    public string? Username { get; set; }
    public string ClientId { get; set; }
    public string Text { get; set; }
}
