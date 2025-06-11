namespace Server.Models.Chat
{
    public class UserChatDTO
    {
        public Guid Id { get; set; }
        public byte[] Avatar { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string LastMessageText { get; set; }
        public DateTime LastMessageSentAt { get; set; }

        public string FullName => $"{Surname} {Name}".Trim();
    }
}
