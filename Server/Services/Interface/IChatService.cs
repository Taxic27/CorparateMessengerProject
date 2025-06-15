using Server.Models.Chat;
using Server.Tools;

namespace Server.Services.Interface
{
    public interface IChatService
    {
        public Result<List<ChatDTO>> GetChatsGroup(Guid currentUserId);
        public Result<List<Guid>> GetAllUsersFromGroup(Guid chatId);
        public Result<List<ChatDTO>> GetChatsPrivate(List<Guid> usersId, Guid currentUserId);
        public Result<ChatDB> FindPrivateChat(Guid selectedUserId, Guid CurrentUserId);
        public Result<ChatDB> CreatePrivateChat(ChatDTO selectedUser, Guid CurrentUserId);
        public Result CreateGroupChat(string groupName, byte[] avatar, List<Guid> memberIds);
        public Result UpdateGroupChat(Guid chatId, string groupName, byte[] avatar, List<Guid> memberIds);
        public Result DeleteChatGroup(Guid chatId);
    }
}
