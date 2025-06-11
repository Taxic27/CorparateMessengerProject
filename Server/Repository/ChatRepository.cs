using Dapper;
using Server.Models.Chat;
using Server.Repository.Interface;
using Server.Tools;
using System;

namespace Server.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly IMainConnector _mainConnector;

        public ChatRepository(IMainConnector mainConnector)
        {
            _mainConnector = mainConnector;
        }

        public List<ChatDTO> GetChatsGroup(Guid userId)
        {
            string request = @"
    SELECT
        c.id,
        c.avatar,
        c.name,
        c.is_group AS IsGroup,
        c.creator,
        CASE
            WHEN last_msg.file_type IS NOT NULL THEN last_msg.file_type
            ELSE last_msg.text
        END AS LastMessageText,
        last_msg.sent_at AS LastMessageSentAt
    FROM chats c
    INNER JOIN chat_users cu ON c.id = cu.chat_id
    LEFT JOIN LATERAL (
        SELECT m.text, m.sent_at, m.file_type
        FROM messages m
        WHERE m.chat_id = c.id
        ORDER BY m.sent_at DESC
        LIMIT 1
    ) AS last_msg ON true
    WHERE cu.user_id = @UserId
    AND c.is_group = true";

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            return _mainConnector.GetList<ChatDTO>(request, parameters);
        }

        public List<Guid> GetAllUsersFromGroup(Guid chatId)
        {
            string request = @"
            SELECT user_id
            FROM chat_users
            WHERE chat_id = @ChatId";

            var parameters = new DynamicParameters();
            parameters.Add("@ChatId", chatId);

            return _mainConnector.GetList<Guid>(request, parameters);
        }

        public List<UserChatDTO> GetChatsPrivate(List<Guid> usersId, Guid currentUserId)
{
            string request = @"
WITH 
chat_data AS (
    SELECT
        cu_other.user_id AS other_user_id,
        CASE
            WHEN last_msg.file_type IS NOT NULL THEN last_msg.file_type
            ELSE last_msg.text
        END AS last_message_text,
        last_msg.sent_at AS last_message_sent_at
    FROM chats c
    JOIN chat_users cu_other ON c.id = cu_other.chat_id
    JOIN chat_users cu_current ON c.id = cu_current.chat_id AND cu_current.user_id = @CurrentUserId
    LEFT JOIN LATERAL (
        SELECT m.text, m.sent_at, m.file_type
        FROM messages m
        WHERE m.chat_id = c.id
        ORDER BY m.sent_at DESC
        LIMIT 1
    ) AS last_msg ON true
    WHERE c.is_group = false
    AND cu_other.user_id = ANY(@OtherUserIds)
)
SELECT
    u.id,
    u.avatar,
    u.name,
    u.surname,
    cd.last_message_text AS LastMessageText,
    cd.last_message_sent_at AS LastMessageSentAt
FROM users u
LEFT JOIN chat_data cd ON u.id = cd.other_user_id
WHERE u.id = ANY(@OtherUserIds)";

            var parameters = new DynamicParameters();
    parameters.Add("@CurrentUserId", currentUserId);
    parameters.Add("@OtherUserIds", usersId);

    return _mainConnector.GetList<UserChatDTO>(request, parameters);
}

        public ChatDB FindPrivateChat(Guid selectedUserId, Guid currentUserId)
        {
            string request = @"
            SELECT
            c.id,
            c.name,
            c.is_group AS IsGroup,
            c.created_at AS CreatedAt,
            u.avatar AS Avatar
            FROM chats c
            INNER JOIN chat_users cu1 ON c.id = cu1.chat_id
            INNER JOIN chat_users cu2 ON c.id = cu2.chat_id
            INNER JOIN users u ON u.id = @User2Id
            WHERE c.is_group = false
            AND cu1.user_id = @User1Id
            AND cu2.user_id = @User2Id";

            var parameters = new DynamicParameters();
            parameters.Add("@User1Id", selectedUserId);
            parameters.Add("@User2Id", currentUserId);

            return _mainConnector.Get<ChatDB>(request, parameters);
        }

        public void AddChat(ChatDB chat)
        {
            string request = @"
            INSERT INTO chats (id, name, is_group, created_at, avatar, creator)
            VALUES (@Id, @Name, @IsGroup, @CreatedAt, @Avatar, @Creator)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", chat.Id);
            parameters.Add("@Name", chat.Name);
            parameters.Add("@IsGroup", chat.IsGroup);
            parameters.Add("@CreatedAt", chat.CreatedAt);
            parameters.Add("@Avatar", chat.Avatar);
            parameters.Add("@Creator", chat.Creator);

            _mainConnector.Execute(request, parameters);
        }

        public void UpdateGroupChat(ChatDB chat)
        {
            string request = @"
            UPDATE chats
            SET name = @Name, avatar = @Avatar
            WHERE id = @Id;
            DELETE FROM chat_users
            WHERE chat_id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", chat.Id);
            parameters.Add("@Name", chat.Name);
            parameters.Add("@Avatar", chat.Avatar);
            _mainConnector.Execute(request, parameters);
        }

        public void AddUserToChat(Guid chatId, Guid userId)
        {
            string request = @"
            INSERT INTO chat_users (chat_id, user_id)
            VALUES (@ChatId, @UserId)
            ON CONFLICT (chat_id, user_id)
            DO NOTHING";

            var parameters = new DynamicParameters();
            parameters.Add("@ChatId", chatId);
            parameters.Add("@UserId", userId);

            _mainConnector.Execute(request, parameters);
        }

        public void AddUsersToChat(Guid chatId, IEnumerable<Guid> userIds)
        {
            if (!userIds.Any()) return;

            string request = @"
                    INSERT INTO chat_users (chat_id, user_id)
                    SELECT @ChatId, unnest(@UserIds)
                    ON CONFLICT (chat_id, user_id) DO NOTHING";

            var parameters = new DynamicParameters();
            parameters.Add("@ChatId", chatId);
            parameters.Add("@UserIds", userIds.ToArray());

            _mainConnector.Execute(request, parameters);
        }
    }
}
