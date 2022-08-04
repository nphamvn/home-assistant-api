using HomeAssistant.API.Entities;
using HomeAssistant.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HomeAssistant.API.Models;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhotoUrl { get; set; }
    public ICollection<Conversation> Conversations { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<UserConversation> UserConversations { get; set; }
}