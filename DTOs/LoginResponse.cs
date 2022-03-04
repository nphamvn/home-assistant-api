namespace HomeAssistant.API.Models;

public class LoginResponse
{
    public string Username { get; set; }
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}