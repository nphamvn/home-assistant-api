using HomeAssistant.API.Data;
using HomeAssistant.API.Entities;
using HomeAssistant.API.Models;

namespace HomeAssistant.API.Services;

public class UserRepository : Repository<AppUser>
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext, dbContext.Users)
    {
    }
}