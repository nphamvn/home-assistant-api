using HomeAssistant.API.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.API.Services.Chat;

public class UserConnectedEventHandler : IConsumer<UserConnected>
{
    private readonly ConnectionManager _connectionManager;
    private readonly MessageService _messageService;
    private readonly IHubContext<ChatHub> _hubContext;

    public UserConnectedEventHandler(ConnectionManager connectionManager,
    MessageService messageService, IHubContext<ChatHub> hubContext)
    {
        _connectionManager = connectionManager;
        _messageService = messageService;
        _hubContext = hubContext;
    }
    public async Task Consume(ConsumeContext<UserConnected> context)
    {
        //Store user connection
        await _connectionManager.Add(context.Message.Username, context.Message.ConnectionId);

        // Get pending messages for user
        var pendingMessages = await _messageService.GetPendingMessage(context.Message.Username);
        if (pendingMessages.Count > 0)
        {
            // Send pending messages to user
            foreach (var message in pendingMessages)
            {
                await _hubContext.Clients.Group(context.Message.Username).SendAsync("ReceiveMessage", message);
            }
        }

        // Inform contact online
    }
}