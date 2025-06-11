using Dapper;
using Server.Models.Message;
using Server.Repository.Interface;
using Server.Tools;
using System.Data.Common;

namespace Server.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMainConnector _mainConnector;

        public MessageRepository(IMainConnector mainConnector)
        {
            _mainConnector = mainConnector;
        }

        public MessageDTO SaveMessageAndReturning(MessageDB message)
        {
            const string request = @"
        WITH inserted_message AS (
            INSERT INTO messages (id, chat_id, sender_id, text, sent_at)
            VALUES (@Id, @ChatId, @SenderId, @Text, @SentAt)
            RETURNING *
        )
        SELECT 
            m.id, 
            m.chat_id as ChatId,
            m.text,
            m.sent_at as SentAt,
            m.file_url as FileUrl,
            m.file_name as FileName,
            m.file_type as FileType,
            m.file_size as FileSize,
            u.username as Username,
 (u.name || ' ' || u.surname) AS SenderName
        FROM inserted_message m
        JOIN users u ON m.sender_id = u.id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@ChatId", message.ChatId);
            parameters.Add("@SenderId", message.SenderId);
            parameters.Add("@Text", message.Text);
            parameters.Add("@SentAt", DateTime.UtcNow);

            return _mainConnector.Get<MessageDTO>(request, parameters);
        }

        public MessageDTO SaveMessageFileAndReturning(MessageDB message)
        {
            const string request = @"
WITH inserted_message AS (
    INSERT INTO messages 
        (id, chat_id, sender_id, text, sent_at, file_url, file_name, file_type, file_size)
    VALUES 
        (@Id, @ChatId, @SenderId, @Text, @SentAt, @FileUrl, @FileName, @FileType, @FileSize)
    RETURNING *
)
SELECT 
    m.id, 
    m.chat_id as ChatId,
    m.text,
    m.sent_at as SentAt,
    m.file_url as FileUrl,
    m.file_name as FileName,
    m.file_type as FileType,
    m.file_size as FileSize,
    u.username as Username,
 (u.name || ' ' || u.surname) AS SenderName
FROM inserted_message m
JOIN users u ON m.sender_id = u.id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", message.Id);
            parameters.Add("@ChatId", message.ChatId);
            parameters.Add("@SenderId", message.SenderId);
            parameters.Add("@Text", message.Text ?? string.Empty);
            parameters.Add("@SentAt", message.SentAt);
            parameters.Add("@FileUrl", message.FileUrl);
            parameters.Add("@FileName", message.FileName);
            parameters.Add("@FileType", message.FileType);
            parameters.Add("@FileSize", message.FileSize);

            return _mainConnector.Get<MessageDTO>(request, parameters);
        }

        public List<MessageDTO> GetMessagesForChat(Guid chatId, int skip, int take)
        {
            const string request = @"
    SELECT 
        m.id, 
        m.chat_id AS ChatId,
        m.text AS Text,
        m.sent_at AS SentAt,
        m.file_url as FileUrl,
        m.file_name as FileName,
            m.file_type as FileType,
            m.file_size as FileSize,
         u.username AS Username,
     (u.name || ' ' || u.surname) AS SenderName
    FROM messages m
    JOIN users u ON m.sender_id = u.id
    WHERE m.chat_id = @ChatId
    ORDER BY m.sent_at DESC
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("@ChatId", chatId);
            parameters.Add("@Skip", skip);
            parameters.Add("@Take", take);

            return _mainConnector.GetList<MessageDTO>(request, parameters);
        }
    }
}
