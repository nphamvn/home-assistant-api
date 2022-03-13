using HomeAssistant.API.Models;

namespace Account.API.Models;

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public Guid MemberId { get; set; }
    public AppUser User { get; set; }
}