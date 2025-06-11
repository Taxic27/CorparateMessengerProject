using Server.Models.Message;
using Server.Repository;
using Server.Repository.Interface;
using Server.Services.Interface;

namespace Server.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public MessageDTO SaveMessage(Guid chatId, Guid senderId, string text)
        {
            var message = new MessageDB
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                SenderId = senderId,
                Text = text,
                SentAt = DateTime.UtcNow
            };

            return _messageRepository.SaveMessageAndReturning(message);
        }

        public MessageDTO SaveMessageFile(Guid chatId, Guid senderId, string fileUrl, string fileName, string fileType, byte[] fileData)
        {
            var message = new MessageDB
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                SenderId = senderId,
                FileUrl = fileUrl,
                FileName = fileName,
                FileType = fileType,
                FileSize = fileData,
                SentAt = DateTime.UtcNow
            };

            return _messageRepository.SaveMessageFileAndReturning(message);
        }

        public List<MessageDTO> GetMessagesForChat(Guid chatId, int skip, int take)
        {
            return _messageRepository.GetMessagesForChat(chatId, skip, take);
        }
    }
}
