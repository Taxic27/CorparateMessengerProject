using Server.Models.User;

namespace Server.Repository.Interface
{
    public interface IUserRepository
    {
        public void SaveUser(UserBlank.Validated validatedUser);
        public UserDB GetUserLogin(string login);
        public UserDB GetUser(string login, string password);
        public List<Guid> GetAllUsersId();
        public List<UserDTO> GetAllUsersExpectCurrent(Guid currentUserId);
    }
}
