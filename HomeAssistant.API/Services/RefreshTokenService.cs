using HomeAssistant.API.Data;
using HomeAssistant.API.Models;

namespace HomeAssistant.API.Services;

public class RefreshTokenService
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenService(ApplicationDbContext context)
    {
        this._context = context;
    }

    public string GenerateRefreshToken(AppUser user)
    {
        var randomNumber = new byte[32];
        //using var generator = new RNGCryptoServiceProvider();
        return "";
    }
}