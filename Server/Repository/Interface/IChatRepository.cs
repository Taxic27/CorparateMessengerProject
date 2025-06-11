using Server.Models.Chat;
using Server.Tools;

namespace Server.Repository.Interface
{
    public interface IChatRepository
    {
        public List<ChatDTO> GetChatsGroup(Guid userId);
        public List<Guid> GetAllUsersFromGroup(Guid chatId);
        public List<UserChatDTO> GetChatsPrivate(List<Guid> usersId, Guid currentUserId);
        public ChatDB FindPrivateChat(Guid selectedUserId, Guid currentUserId);
        public void AddChat(ChatDB chat);
        public void UpdateGroupChat(ChatDB chat);
        public void AddUserToChat(Guid chatId, Guid userId);
        public void AddUsersToChat(Guid chatId, IEnumerable<Guid> userIds);
    }
}
