using HomeAssistant.API.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.API.Services.Chat;

public class UserDisconnectedEventHandler : IConsumer<UserDisconnected>
{
    private readonly ConnectionManager _connectionManager;
    private readonly IHubContext<ChatHub> _hubContext;

    public UserDisconnectedEventHandler(ConnectionManager connectionManager, IHubContext<ChatHub> hubContext)
    {
        _connectionManager = connectionManager;
        _hubContext = hubContext;
    }
    public async Task Consume(ConsumeContext<UserDisconnected> context)
    {
        Console.WriteLine("User disconnected: " + context.Message.Username);
        //Remove user connection
        _connectionManager.Remove(context.Message.Username, context.Message.ConnectionId);

        var existingConnections = await _connectionManager.Get(context.Message.Username);
        Console.WriteLine("UserDisconnectedEventHandler: " + context.Message.Username + " has " + existingConnections.Count + " connections");
        // Inform contact offline
        if (existingConnections.Count == 0)
        {
            //Inform contact offline to others
            await _hubContext.Clients.AllExcept(existingConnections).SendAsync("UserOffline", context.Message.Username);
        }
    }
}