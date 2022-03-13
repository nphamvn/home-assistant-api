using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomeAssistant.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeAssistant.API.Services;

public class TokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtOptions> options, UserManager<AppUser> userManager, ILogger<TokenService> logger)
    {
        _jwtOptions = options.Value;
        _userManager = userManager;
        this._logger = logger;
    }
    public async Task<(string Token, DateTime expiration)> GenerateToken(AppUser member)
    {
        var claims = await _userManager.GetClaimsAsync(member);
        claims.Add(new Claim("name", member.UserName));

        var roles = await _userManager.GetRolesAsync(member);
        foreach (var role in roles)
        {
            claims.Add(new Claim("role", role));
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        DateTime expiration = DateTime.UtcNow.AddMinutes(_jwtOptions.TokenExpirationMinutes);
        JwtSecurityToken securityToken = new(_jwtOptions.Issuer, _jwtOptions.Audience,
            claims,
            DateTime.UtcNow,
            expiration,
            credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return (token, expiration);
    }
    public async Task<string> RefreshToken()
    {
        return "token";
    }
}