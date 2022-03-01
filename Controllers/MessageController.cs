using HomeAssistant.API.Data;
using HomeAssistant.API.Entities;
using HomeAssistant.API.Services;
using HomeAssistant.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IRepository<Message> _messageRepository;
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly ApplicationDbContext _context;

    public MessageController(IRepository<Message> messageRepository,
                    IRepository<Conversation> conversationRepository,
                    ApplicationDbContext context)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> CreateMessage()
    {
        var senderUsername = IdentityService.GetUsername(User);
        var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserName == senderUsername);

        var conversations = await _conversationRepository.Find(c => c.Creator.UserName == senderUsername && c.Partner.UserName == "yenanh");
        var conversation = conversations.FirstOrDefault();
        var message = new Message()
        {
            ConversationId = conversation.Id,
            Conversation = conversation,
            SenderId = sender.Id,
            Sender = sender,
            Text = "Hello"
        };

        await _messageRepository.Create(message);

        return Ok();
    }

    [HttpGet("/{conversationId}")]
    public async Task<IActionResult> GetMessages(int conversationId)
    {
        //For "Inbox" tab in chat module
        var username = IdentityService.GetUsername(User);
        var messages = await _messageRepository.Find(m => m.ConversationId == conversationId);
        return Ok(messages);
    }
}