namespace HomeAssistant.API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string SenderUsername { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public string Username { get; set; }
    }
}