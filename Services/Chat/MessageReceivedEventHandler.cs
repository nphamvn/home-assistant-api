using HomeAssistant.API.Entities;
using HomeAssistant.API.Hubs;
using HomeAssistant.API.Models;
using HomeAssistant.API.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace HomeAssistant.API.Services.Chat;

public class MessageReceivedEventHandler : IConsumer<ReceivedMessage>
{
    private readonly IRepository<Conversation> _conversationRepository;
    private readonly IRepository<Message> _messageRepository;
    private readonly IRepository<AppUser> _userRepository;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ConnectionManager _connectionManager;

    public MessageReceivedEventHandler(IRepository<AppUser> userRepository,
    IRepository<Message> messageRepository, IRepository<Conversation> conversationRepository,
    IHubContext<ChatHub> hubContext, ConnectionManager connectionManager)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _hubContext = hubContext;
        _connectionManager = connectionManager;
    }
    public async Task Consume(ConsumeContext<ReceivedMessage> context)
    {
        var message = context.Message;
        var sender = await _userRepository.Single(u => u.UserName == message.SenderUsername);

        if (message.ConversationId == null && message.RecipientUsername != null)
        {
            var partner = await _userRepository.Single(u => u.UserName == message.RecipientUsername);
            var conversation = new Conversation()
            {
                Description = "Conversation between " + sender.UserName + " and " + partner.UserName,
                Creator = sender,
                Partner = partner,
                ConversationNames = new List<ConversationName>()
                {
                    new ConversationName(){User = sender},
                    new ConversationName(){User = partner}
                }
            };

            await _conversationRepository.Create(conversation);
            message.ConversationId = conversation.Id;
        }
        else
        {
            var conversation = await _conversationRepository.Single(c => c.Id == message.ConversationId, c => c.Partner);

            //RecipientUsername can be Creator or Partner in Conversation
            //PartnerUsername must not be the same as the SenderUsername
            if (message.SenderUsername == conversation.Partner.UserName)
            {
                conversation = await _conversationRepository.Single(c => c.Id == message.ConversationId, c => c.Creator);
                message.RecipientUsername = conversation.Creator.UserName;
            }
            else
            {
                message.RecipientUsername = conversation.Partner.UserName;
            }
            //TODO: Get other user(s) from conversation
            //message.RecipientUsername = conversation.Partner.UserName;
        }

        await _messageRepository.Create(new Message()
        {
            ConversationId = (int)message.ConversationId,
            SenderId = sender.Id,
            Text = message.Text
        });

        // Inform that message was sent to server   
        await _hubContext.Clients.Group(sender.UserName).SendAsync("MessageSent", message);

        //Get count of connections of partner
        var connectionIds = await _connectionManager.Get(message.RecipientUsername);
        if (connectionIds?.Count >= 1)
        {
            // Send message to recipient
            await _hubContext.Clients.Group(message.RecipientUsername).SendAsync("ReceiveMessage", message.ConversationId, message.Text);
            Console.WriteLine("Message sent to recipient");
            // Inform that message was sent to recipient
            await _hubContext.Clients.Group(sender.UserName).SendAsync("MessageDelivered", message);
        }
        else
        {
            //TODO: Send to offline message
        }
    }
}