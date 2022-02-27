using Microsoft.AspNetCore.Identity;

namespace HomeAssistant.API.Models;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}