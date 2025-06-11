namespace Server.Models.Chat
{
    public class ChatDB
    {
        public Guid Id { get; set; }
        public byte[] Avatar { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid Creator { get; set; }
    }
}
