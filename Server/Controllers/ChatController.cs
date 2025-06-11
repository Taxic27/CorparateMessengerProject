using Microsoft.AspNetCore.Mvc;
using Server.Models.Chat;
using Server.Services;
using Server.Services.Interface;
using Server.Tools;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/chats")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("userchats-group")]
        public Result<List<ChatDTO>> GetUserChats(Guid currentUserId)
        {
            var result = _chatService.GetChatsGroup(currentUserId);
            return result.IsSuccess
                ? Result<List<ChatDTO>>.Success(result.Data!)
                : Result<List<ChatDTO>>.Fail(result.ErrorsAsString);
        }

        [HttpGet("alluserchats-group")]
        public Result<List<Guid>> GetAllUsersFromGroup(Guid chatId)
        {
            var result = _chatService.GetAllUsersFromGroup(chatId);
            return result.IsSuccess
                ? Result<List<Guid>>.Success(result.Data!)
                : Result<List<Guid>>.Fail(result.ErrorsAsString);
        }

        public record UpdateChatRequest(Guid chatId ,string GroupName, byte[] Avatar, List<Guid> MemberIds);

        [HttpPut("updatechat-group")]
        public Result UpdateChatGroup(UpdateChatRequest request)
        {
            return _chatService.UpdateGroupChat(request.chatId ,request.GroupName, request.Avatar, request.MemberIds);
        }

        public record UserChatsRequest (List<Guid> UsersId, Guid CurrentUserId);

        [HttpPost("userchats-private")]
        public Result<List<ChatDTO>> GetPrivateUserChats([FromBody] UserChatsRequest request)
        {
            var result = _chatService.GetChatsPrivate(request.UsersId, request.CurrentUserId);

            return result.IsSuccess
                ? Result<List<ChatDTO>>.Success(result.Data!)
                : Result<List<ChatDTO>>.Fail(result.ErrorsAsString);
        }

        public record CreateChatRequest (string GroupName, byte[] Avatar, List<Guid> MemberIds);

        [HttpPost("createchat-group")]
        public Result CreateChatGroup([FromBody] CreateChatRequest request)
        {
           return _chatService.CreateGroupChat(request.GroupName, request.Avatar, request.MemberIds);
        }

        [HttpGet("private-exists")]
        public Result<ChatDB> CheckPrivateChatExists(Guid selectedUserId, Guid currentUserId)
        {
            var result = _chatService.FindPrivateChat(selectedUserId, currentUserId);
            return result.Data != null
                ? Result<ChatDB>.Success(result.Data)
                : Result<ChatDB>.Fail(result.ErrorsAsString);
        }

        public record PrivateChatRequest(ChatDTO SelectedUser, Guid CurrentUserId);

        [HttpPost("private")]
        public Result<ChatDB> CreatePrivateChat([FromBody] PrivateChatRequest request)
        {
            var result = _chatService.CreatePrivateChat(request.SelectedUser, request.CurrentUserId);
            return result.IsSuccess
                ? Result<ChatDB>.Success(result.Data!)
                : Result<ChatDB>.Fail(result.ErrorsAsString);
        }
    }
}
