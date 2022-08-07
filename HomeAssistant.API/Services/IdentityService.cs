using System.Security.Claims;

namespace HomeAssistant.API.Services;

public class IdentityService
{
    public static string? GetUsername(ClaimsPrincipal claims)
    {
        return claims.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
    }
}