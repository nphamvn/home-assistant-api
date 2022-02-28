using HomeAssistant.API.Models;

namespace HomeAssistant.API.Entities;

public class Message
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string SenderId { get; set; }
    public AppUser Sender { get; set; }
    public string RecipientId { get; set; }
    public AppUser Recipient { get; set; }
}