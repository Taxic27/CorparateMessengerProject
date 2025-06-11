using Server.Models.User;
using Server.Tools;

namespace Server.Services.Interface
{
    public interface IUserService
    {
        public Result<UserDB> Login(string? login, string? password);
        public Result<List<Guid>> GetAllUsersId();
        public Result<List<UserDTO>> GetAllUsersExpectCurrent(Guid currentUserId);
    }
}
