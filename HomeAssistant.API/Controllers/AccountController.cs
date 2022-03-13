using HomeAssistant.API.Data;
using HomeAssistant.API.DTOs;
using HomeAssistant.API.Models;
using HomeAssistant.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<AppUser> _userManager;
    public AccountController(ApplicationDbContext context, SignInManager<AppUser> signInManager,
                            TokenService tokenService, RefreshTokenService refreshTokenService,
                            UserManager<AppUser> userManager, ILogger<AccountController> logger)
    {
        _context = context;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("ping")]
    public async Task<ActionResult> Ping()
    {
        return Ok("pong");
    }

    [HttpGet("token")]
    public async Task<ActionResult> GetUser()
    {
        //var username = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
        var username = IdentityService.GetUsername(User);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        var role = await _userManager.GetRolesAsync(user);
        //Generate Refresh Token
        var refreshToken = _refreshTokenService.GenerateRefreshToken(user);

        //Generate token
        var (token, expiration) = await _tokenService.GenerateToken(user);

        return Ok(new
        {
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = role,
            AccessToken = token,
            AccessTokenExpiration = expiration,
            RefreshToken = refreshToken
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        _logger.LogInformation("User login");
        AppUser user = null;
        if (!string.IsNullOrEmpty(loginRequest.Username))
        {
            //Login with Username
            user = await _userManager.FindByNameAsync(loginRequest.Username);
            //user = await _context.AppUsers.SingleOrDefaultAsync(m => m.UserName.ToUpper() == loginRequest.Username.ToUpper());

        }
        else if (!string.IsNullOrEmpty(loginRequest.Email))
        {
            //Login with Email
            user = await _userManager.FindByEmailAsync(loginRequest.Email);
            //user = await _context.AppUsers.SingleOrDefaultAsync(m => m.Email == loginRequest.Email);
        }
        else
        {
            return BadRequest("No login information");
        }
        if (user == null)
        {
            return BadRequest("User does not exists");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, false);
        if (signInResult.Succeeded)
        {
            var role = await _userManager.GetRolesAsync(user);
            //Generate Refresh Token
            var refreshToken = _refreshTokenService.GenerateRefreshToken(user);

            //Generate token
            var (token, expiration) = await _tokenService.GenerateToken(user);

            return Ok(new
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = role,
                AccessToken = token,
                AccessTokenExpiration = expiration,
                RefreshToken = refreshToken
            });
        }
        else
        {
            return BadRequest("Login failed");
        }

    }

    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest registerRequest)
    {
        AppUser user = null;
        user = await _userManager.FindByNameAsync(registerRequest.Username);
        if (user != null)
        {
            return BadRequest("User already exists");
        }
        user = await _userManager.FindByEmailAsync(registerRequest.Email);
        if (user != null)
        {
            return BadRequest("Email already exists");
        }
        user = new AppUser
        {
            UserName = registerRequest.Username,
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName
        };
        var result = await _userManager.CreateAsync(user, registerRequest.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, AppRole.USER_ROLE);
            //Generate Refresh Token
            var refreshToken = _refreshTokenService.GenerateRefreshToken(user);

            //Generate token
            var (token, expiration) = await _tokenService.GenerateToken(user);

            return Ok(new
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = AppRole.USER_ROLE,
                AccessToken = token,
                AccessTokenExpiration = expiration,
                RefreshToken = refreshToken
            });
        }
        else
        {
            return BadRequest("Registration failed: " + result.Errors.Select(e => e.Description));
        }
    }

    [HttpGet]
    [Route("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var username = IdentityService.GetUsername(User);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        //var role = await _userManager.GetRolesAsync(user);
        return Ok(new
        {
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
            //Role = role
        });
    }

    [HttpPost]
    [Route("profile")]
    public async Task<IActionResult> GetProfile([FromBody] UpdateProfileRequest request)
    {
        var username = IdentityService.GetUsername(User);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        //user.Email = request.Email;
        await _context.SaveChangesAsync();
        return Ok(new
        {
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        });
    }
    [HttpPost]
    [Route("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequest request)
    {
        var username = IdentityService.GetUsername(User);
        var user = await _userManager.FindByNameAsync(username);
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (result.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] UpdatePasswordRequest request)
    {
        return Ok();
    }
}