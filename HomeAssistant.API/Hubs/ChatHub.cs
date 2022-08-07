using HomeAssistant.API.Data;
using HomeAssistant.API.Hubs.DTOs;
//using HomeAssistant.API.DTOs;
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
    public ChatHub(ILogger<ChatHub> logger, IPublishEndpoint publishEndpoint)
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

        _logger.LogInformation($"{username} connected to ChatHub");
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
        _logger.LogInformation($"{username} disconnected from ChatHub");
        await base.OnDisconnectedAsync(exception);
    }

    // public async Task SendMessage(int? conversationId, string? partnerUsername, string message)
    // {
    //     var senderUsername = IdentityService.GetUsername(Context.User) ?? throw new ArgumentNullException("senderUsername cannot be null");
    //     if (conversationId != null || partnerUsername != null)
    //     {
    //         await _publishEndpoint.Publish<ReceivedMessage>(new ReceivedMessage()
    //         {
    //             ConversationId = conversationId,
    //             RecipientUsername = partnerUsername,
    //             SenderUsername = senderUsername,
    //             Text = message
    //         });
    //     }
    // }

    public async Task SendMessage(MessageDto message)
    {
        var senderUsername = IdentityService.GetUsername(Context.User) ?? throw new ArgumentNullException("senderUsername cannot be null");
        if (message.ConversationId != null || message.Username != null)
        {
            await _publishEndpoint.Publish<ReceivedMessage>(new ReceivedMessage()
            {
                ConversationId = message.ConversationId,
                RecipientUsername = message.Username,
                SenderUsername = senderUsername,
                ClientId = message.ClientId,
                Text = message.Text
            });
        }
    }

    public async Task SendMessageTyping(int conversationId)
    {
        var senderUsername = IdentityService.GetUsername(Context.User) ?? throw new ArgumentNullException("senderUsername cannot be null");
        await _publishEndpoint.Publish<MessageTyping>(new MessageTyping()
        {
            Username = senderUsername,
            ConversationId = conversationId
        });
    }
}