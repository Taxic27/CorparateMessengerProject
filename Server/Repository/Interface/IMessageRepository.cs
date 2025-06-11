using Server.Models.Message;

namespace Server.Repository.Interface
{
    public interface IMessageRepository
    {
        public MessageDTO SaveMessageAndReturning(MessageDB message);
        public MessageDTO SaveMessageFileAndReturning(MessageDB message);
        public List<MessageDTO> GetMessagesForChat(Guid chatId, int skip = 0, int take = 50);
    }
}
