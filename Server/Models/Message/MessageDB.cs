namespace Server.Models.Message
{
    public class MessageDB
    {
        public Guid Id { get; set; }          
        public Guid ChatId { get; set; }   
        public Guid SenderId { get; set; }   
        public string? Text { get; set; }     
        public DateTime SentAt { get; set; }  
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public byte[]? FileSize { get; set; }
    }
}
