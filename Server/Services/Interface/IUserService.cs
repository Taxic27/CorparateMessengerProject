using Server.Models.User;
using Server.Tools;

namespace Server.Services.Interface
{
    public interface IUserService
    {
        public Result<UserDB> Login(string? login, string? password);
        public Result<List<Guid>> GetAllUsersId();
        public Result CreateUser(string username, string password, string name, string surname,
                  string? patronymic,
                  string currentPosition,
                  byte[]? avatar);
        public Result UpdateUser(Guid userid, string name, string surname, string? patronymic,
                 string currentPosition,
                 byte[]? avatar);
        public Result<List<UserDTO>> GetAllUsersExpectCurrent(Guid currentUserId);
    }
}
