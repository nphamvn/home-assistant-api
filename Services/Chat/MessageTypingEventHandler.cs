using HomeAssistant.API.Hubs;
using MassTransit;

namespace HomeAssistant.API.Services.Chat;

public class MessageTypingEventHandler : IConsumer<MessageTyping>
{
    public Task Consume(ConsumeContext<MessageTyping> context)
    {
        throw new NotImplementedException();
    }
}