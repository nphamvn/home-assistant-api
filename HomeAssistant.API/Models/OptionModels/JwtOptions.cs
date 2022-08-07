namespace HomeAssistant.API.Models;

public class JwtOptions
{
    public string Secret { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int TokenExpirationMinutes { get; set; }
}