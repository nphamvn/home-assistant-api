using HomeAssistant.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }
    public override async Task OnConnectedAsync()
    {
        var username = IdentityService.GetUsername(Context.User);
        _logger.LogInformation($"{username} connected");
        await Groups.AddToGroupAsync(Context.ConnectionId, username);
        //await Clients.All.SendAsync("ReceiveMessage", $"{username} connected");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string user, string message)
    {
        var senderUsername = IdentityService.GetUsername(Context.User);
        _logger.LogInformation($"{senderUsername} sent {user} message: {message}");
        await Clients.Group(user).SendAsync("ReceiveMessage", senderUsername, message);
        //await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}