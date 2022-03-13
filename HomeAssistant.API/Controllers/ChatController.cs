using AutoMapper;
using HomeAssistant.API.Data;
using HomeAssistant.API.DTOs;
using HomeAssistant.API.Entities;
using HomeAssistant.API.Models;
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
    private readonly IRepository<AppUser> _userRepository;
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IRepository<Message> _messageRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public ChatController(IRepository<AppUser> userRepository,
                    IRepository<Conversation> conversationRepository,
                    IRepository<Message> messageRepository,
                    IMapper mapper,
                    ApplicationDbContext context)
    {
        _userRepository = userRepository;
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
        _context = context;
    }

    [HttpGet()]
    [Route("conversation")]
    public async Task<IActionResult> GetConversation([FromQuery] int? id, [FromQuery] string? participant)
    {
        var username = IdentityService.GetUsername(User);
        if (id != null)
        {
            var conversation = await _conversationRepository.Single(c => c.Id == id);

            return Ok(conversation);
        }
        else if (participant != null)
        {
            var conversation = await _conversationRepository.Single(c => c.Creator.UserName == participant);
            return Ok(conversation);
        }
        else
        {
            var conversations = await _conversationRepository
                    .Find(c => c.Creator.UserName == username);// || c.Partner.UserName == username);
            return Ok(conversations);
        }
    }

    [HttpGet()]
    [Route("conversation/{id}/messages")]
    public async Task<IActionResult> GetConversationMessages(int id)
    {
        //var conversation = await _conversationRepository.Single(c => c.Id == conversationId, c => c.Messages);
        //var messages = await _context.Conversations.
        var messages = await _context.Messages.Where(m => m.ConversationId == id)
                                            //.Include(m => m.Sender)
                                            .ToListAsync();
        //Include(m => m.Sender).ToListAsync();

        //var messages = conversation.Messages;
        var ret = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            Text = m.Text,
            SenderUsername = m.Sender.UserName,
        });

        return Ok(messages);
    }

    [HttpGet]
    [Route("contact")]
    public async Task<IActionResult> GetContacts()
    {
        var username = IdentityService.GetUsername(User);

        var others = await _userRepository.Find(u => u.UserName != username);
        //others
        var ret = _mapper.Map<IEnumerable<ContactDto>>(others);

        return Ok(ret);
    }

    [HttpGet]
    [Route("contact/{username}")]
    public async Task<IActionResult> GetContact(string username)
    {
        var user = await _userRepository.Single(u => u.UserName == username);
        return Ok(user);
    }
}