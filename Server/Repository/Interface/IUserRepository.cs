using Server.Models.User;

namespace Server.Repository.Interface
{
    public interface IUserRepository
    {
        public UserDB GetUser(string login, string password);
        public List<Guid> GetAllUsersId();
        public void CreateUser(UserDB user);
        public void UpdateUser(UserDB user);
        public List<UserDTO> GetAllUsersExpectCurrent(Guid currentUserId);
    }
}
