namespace HomeAssistant.API.Services.Chat;

public class ReceivedMessage
{
    public int? ConversationId { get; set; }
    public string SenderUsername { get; set; }
    public string? RecipientUsername { get; set; }
    public string? Text { get; set; }
}