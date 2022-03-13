namespace HomeAssistant.API.DTOs;

public class ContactDto
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? ConversationId { get; set; }
    public string? ConversationName { get; set; }
}