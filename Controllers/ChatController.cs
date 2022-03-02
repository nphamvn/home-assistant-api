using HomeAssistant.API.Data;
using HomeAssistant.API.DTOs;
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
public class ChatController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<Conversation> _conversationRepository;

    public ChatController(IRepository<Conversation> conversationRepository, ApplicationDbContext context)
    {
        _context = context;

        _conversationRepository = conversationRepository;
    }

    [HttpGet()]
    [Route("conversation")]
    public async Task<IActionResult> GetUserConversations()
    {
        var username = IdentityService.GetUsername(User);

        var conversations = await _conversationRepository.Find(c => c.Creator.UserName == username || c.Partner.UserName == username);

        return Ok(conversations);
    }

    [HttpGet()]
    [Route("conversation/{id}")]
    public async Task<IActionResult> GetConversation(int id)
    {
        var conversation = await _conversationRepository.Find(c => c.Id == id);

        return Ok(conversation);
    }

    [HttpGet()]
    [Route("message")]
    public async Task<IActionResult> GetConversationMessages([FromQuery] int conversationId)
    {
        var conversation = await _conversationRepository.Single(c => c.Id == conversationId);
        var messages = await _context.Messages.Where(m => m.Conversation == conversation)
        .Include(m => m.Sender).ToListAsync();
        //var messages = conversation.Messages;
        var ret = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            Text = m.Text,
            SenderUsername = m.Sender.UserName,
        }).ToList();
        return Ok(ret);
    }

    [HttpGet]
    [Route("contact")]
    public async Task<IActionResult> GetContacts()
    {
        //var username = IdentityService.GetUsername(User);

        //var conversations = await _conversationRepository.Find(c => c.Creator.UserName == username || c.Partner.UserName == username);
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet]
    [Route("contact/{username}")]
    public async Task<IActionResult> GetContact(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        return Ok(user);
    }
}