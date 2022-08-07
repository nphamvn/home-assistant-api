using HomeAssistant.API.Data;
using HomeAssistant.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.API.Data;

public static class Seed
{
    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

        if (await userManager.Users.AnyAsync())
        {
            return;
        }
        await roleManager.CreateAsync(new AppRole { Name = AppRole.ADMIN_ROLE });
        await roleManager.CreateAsync(new AppRole { Name = AppRole.USER_ROLE });

        AppUser user = new AppUser()
        {
            UserName = "admin",
            Email = "admin@home-assistant.io",
            FirstName = "Nam",
            LastName = "Pham",
        };
        await userManager.CreateAsync(user, "P@ssw0rd");
        await userManager.AddToRoleAsync(user, "Admin");
    }
}