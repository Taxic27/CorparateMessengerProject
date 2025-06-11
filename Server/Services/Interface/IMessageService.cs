using Server.Models.Message;

namespace Server.Services.Interface
{
    public interface IMessageService
    {
        public MessageDTO SaveMessage(Guid chatId, Guid senderId, string text);
        public MessageDTO SaveMessageFile(Guid chatId, Guid senderId, string fileUrl, string fileName, string fileType, byte[] fyleData);
        public List<MessageDTO> GetMessagesForChat(Guid chatId, int skip, int take);
    }
}
