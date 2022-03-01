using HomeAssistant.API.Models;
using HomeAssistant.API.Services.Interfaces;

namespace HomeAssistant.API.Entities
{
    public class Conversation : IHasId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatorId { get; set; }
        public AppUser Creator { get; set; }
        public string PartnerId { get; set; }
        public AppUser Partner { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}