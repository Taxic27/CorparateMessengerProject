using Server.Models.Chat;
using Server.Repository.Interface;
using Server.Services.Interface;
using Server.Tools;
using System;

namespace Server.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public Result<List<ChatDTO>> GetChatsGroup(Guid currentUserId)
        {
            var chats = _chatRepository.GetChatsGroup(currentUserId);

            return chats == null
                ? Result<List<ChatDTO>>.Fail("Чаты не найдены")
                : Result<List<ChatDTO>>.Success(chats);
        }

        public Result<List<Guid>> GetAllUsersFromGroup(Guid chatId)
        {
            var userIds = _chatRepository.GetAllUsersFromGroup(chatId);

            return userIds == null
                ? Result<List<Guid>>.Fail("Чаты не найдены")
                : Result<List<Guid>>.Success(userIds);
        }

        public Result<List<ChatDTO>> GetChatsPrivate(List<Guid> usersId, Guid currentUserId)
        {
            var userChats = _chatRepository.GetChatsPrivate(usersId, currentUserId);

            return userChats != null
                ? Result<List<ChatDTO>>.Success(userChats.Select(u => new ChatDTO
                {
                    Id = u.Id,
                    Avatar = u.Avatar,
                    Name = u.FullName,
                    IsGroup = false,
                    LastMessageText = u.LastMessageText,
                    LastMessageSentAt = u.LastMessageSentAt
                }).ToList())
                : Result<List<ChatDTO>>.Fail("Чаты не найдены");
        }

        public Result<ChatDB> FindPrivateChat(Guid selectedUserId, Guid currentUserId)
        {
            var chat = _chatRepository.FindPrivateChat(selectedUserId, currentUserId);

            return chat == null
                ? Result<ChatDB>.Fail("Чат не найден")
                : Result<ChatDB>.Success(chat);
        }

        public Result<ChatDB> CreatePrivateChat(ChatDTO selectedUser, Guid currentUserId)
        {
            var newChat = new ChatDB
            {
                Id = Guid.NewGuid(),
                Name = selectedUser.Name,
                IsGroup = false,
                CreatedAt = DateTime.UtcNow,
                Avatar = null
            };

            _chatRepository.AddChat(newChat);

            _chatRepository.AddUserToChat(newChat.Id, selectedUser.Id);
            _chatRepository.AddUserToChat(newChat.Id, currentUserId);

            return newChat == null
                ? Result<ChatDB>.Fail("Чат не найден")
                : Result<ChatDB>.Success(newChat);
        }

        public Result CreateGroupChat(string groupName, byte[] avatar, List<Guid> memberIds)
        {
            try
            {
                var newChat = new ChatDB
                {
                    Id = Guid.NewGuid(),
                    Name = groupName,
                    IsGroup = true,
                    CreatedAt = DateTime.UtcNow,
                    Avatar = avatar
                };

                _chatRepository.AddChat(newChat);

                _chatRepository.AddUsersToChat(newChat.Id, memberIds);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail("Ошибка при создании чата");
            }
        }

        public Result UpdateGroupChat(Guid chatId ,string groupName, byte[] avatar, List<Guid> memberIds)
        {
            try
            {
                var newChat = new ChatDB
                {
                    Id = chatId,
                    Name = groupName,
                    IsGroup = true,
                    CreatedAt = DateTime.UtcNow,
                    Avatar = avatar
                };

                _chatRepository.UpdateGroupChat(newChat);

                _chatRepository.AddUsersToChat(newChat.Id, memberIds);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail("Ошибка при создании чата");
            }
        }
    }
}
