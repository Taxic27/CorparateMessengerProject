namespace Server.Models.Chat
{
    public class ChatDTO
    {
        public Guid Id { get; set; }
        public byte[] Avatar { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public string? LastMessageText { get; set; }
        public DateTime LastMessageSentAt { get; set; }
    }
}
