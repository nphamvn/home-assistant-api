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
public class ChatController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<Conversation> _conversationRepository;

    public ChatController(IRepository<Conversation> conversationRepository, ApplicationDbContext context)
    {
        _context = context;

        _conversationRepository = conversationRepository;
    }

    // [HttpPost]
    // public async Task<IActionResult> CreateConversation()
    // {
    //     var username = IdentityService.GetUsername(User);
    //     var creator = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    //     var partner = await _context.Users.FirstOrDefaultAsync(u => u.UserName == "yenanh");
    //     var conversation = new Conversation()
    //     {
    //         Name = creator.UserName + "-" + partner.UserName,
    //         Creator = creator,
    //         Partner = partner
    //     };
    //     await _conversationRepository.Create(conversation);
    //     return Ok(new
    //     {
    //         id = conversation.Id,
    //         name = conversation.Name,
    //         creator = conversation.Creator.UserName,
    //         partner = conversation.Partner.UserName
    //     });
    // }

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
        var conversation = await _conversationRepository.Single(c => c.Id == conversationId, c => c.Messages);
        var messages = conversation.Messages;
        foreach (var message in messages)
        {
            Console.WriteLine(message.Text);
        }
        return Ok("return messages");
    }
}