namespace HomeAssistant.API.Hubs
{
    public class UserDisconnected
    {
        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}