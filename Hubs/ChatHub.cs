using HomeAssistant.API.Data;
using HomeAssistant.API.Services;
using HomeAssistant.API.Services.Chat;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private readonly IPublishEndpoint _publishEndpoint;

    private readonly ILogger<ChatHub> _logger;
    public ChatHub(ILogger<ChatHub> logger,
    ApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }
    public override async Task OnConnectedAsync()
    {
        var username = IdentityService.GetUsername(Context.User);
        await _publishEndpoint.Publish<UserConnected>(new UserConnected()
        {
            Username = username,
            ConnectionId = Context.ConnectionId
        });

        _logger.LogInformation($"{username} connected");
        await Groups.AddToGroupAsync(Context.ConnectionId, username);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = IdentityService.GetUsername(Context.User);
        await _publishEndpoint.Publish<UserDisconnected>(new UserDisconnected()
        {
            Username = username,
            ConnectionId = Context.ConnectionId
        });
        _logger.LogInformation($"{username} disconnected");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(int? conversationId, string? partnerUsername, string message)
    {
        if (conversationId == null && partnerUsername == null)
        {
            throw new ArgumentNullException("conversationId and clientConversationId cannot be null at the same time");
        }

        var senderUsername = IdentityService.GetUsername(Context.User) ?? throw new ArgumentNullException("senderUsername cannot be null");

        await _publishEndpoint.Publish<ReceivedMessage>(new ReceivedMessage()
        {
            ConversationId = conversationId,
            RecipientUsername = partnerUsername,
            SenderUsername = senderUsername,
            Text = message
        });
    }
}