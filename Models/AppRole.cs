using Microsoft.AspNetCore.Identity;

namespace HomeAssistant.API.Models;

public class AppRole : IdentityRole
{
    public const string ADMIN_ROLE = "Admin";
    public const string USER_ROLE = "User";
}