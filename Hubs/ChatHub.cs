using HomeAssistant.API.Data;
using HomeAssistant.API.Entities;
using HomeAssistant.API.Services;
using HomeAssistant.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChatHub> _logger;
    public ChatHub(ILogger<ChatHub> logger, IRepository<Conversation> conversationRepository,
    ApplicationDbContext context)
    {
        _logger = logger;
        _conversationRepository = conversationRepository;
        _context = context;
    }
    public override async Task OnConnectedAsync()
    {
        var username = IdentityService.GetUsername(Context.User);
        _logger.LogInformation($"{username} connected");
        await Groups.AddToGroupAsync(Context.ConnectionId, username);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string? conversationId, string? partnerUsername, string message)
    {
        if (conversationId == null && partnerUsername == null)
        {
            throw new ArgumentNullException("conversationId and clientConversationId cannot be null at the same time");
        }
        if (conversationId == null && partnerUsername != null)
        {
            //Conversation does not exist, create it
            var senderUsername = IdentityService.GetUsername(Context.User);
            var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserName == senderUsername);
            var partner = await _context.Users.FirstOrDefaultAsync(u => u.UserName == partnerUsername);
            var conversation = new Conversation()
            {
                Name = senderUsername + "-" + partnerUsername,
                Creator = sender,
                Partner = partner
            };

            await _conversationRepository.Create(conversation);
            conversationId = conversation.Id.ToString();
            _logger.LogInformation($"{senderUsername} created conversation with {partnerUsername}");
        }

        await Clients.All.SendAsync("ReceiveMessage", conversationId, message);
    }
}