using Npgsql;
using Server.Tools;
using Server.Models.User;
using Dapper;
using Server.Repository.Interface;

namespace Server.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMainConnector _mainConnector;

        public UserRepository(IMainConnector mainConnector)
        {
            _mainConnector = mainConnector;
        }

        public void CreateUser(UserDB user)
        {
            string request = @"INSERT INTO users (id, username, password, created_at, name, surname, patronymic, avatar, current_position) 
                    VALUES (@Id, @Login, @Password, @CreateDateTime, @Name, @Surname, @Patronymic, @Avatar, @Current_position)";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", user.Id);
            parameters.Add("@Login", user.Username);
            parameters.Add("@Password", user.Password);
            parameters.Add("@CreateDateTime", user.CreatedAt);
            parameters.Add("@Name", user.Name);
            parameters.Add("@Surname", user.Surname);
            parameters.Add("@Patronymic", user.Patronymic);
            parameters.Add("@Avatar", user.Avatar);
            parameters.Add("@Current_position", user.CurrentPosition);

            _mainConnector.Execute(request, parameters);
        }

        public void UpdateUser(UserDB user)
        {
            string request = @"UPDATE users 
                       SET name = @Name, 
                           surname = @Surname, 
                           patronymic = @Patronymic, 
                           avatar = @Avatar, 
                           current_position = @Current_position 
                       WHERE id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("@Id", user.Id);
            parameters.Add("@Name", user.Name);
            parameters.Add("@Surname", user.Surname);
            parameters.Add("@Patronymic", user.Patronymic);
            parameters.Add("@Avatar", user.Avatar);
            parameters.Add("@Current_position", user.CurrentPosition);
            _mainConnector.Execute(request, parameters);
        }

        public UserDB GetUser(string login, string password)
        {
            string request = @"SELECT 
                             id, 
                             avatar, 
                             name, 
                             surname, 
                             patronymic,
                             username,
                             role,
                             current_position AS CurrentPosition
                             FROM users 
                             WHERE username = @Login AND password = @Password";

            var parameters = new DynamicParameters();
            parameters.Add("@Login", login);
            parameters.Add("@Password", password);

            UserDB user = _mainConnector.Get<UserDB>(request, parameters);
            return user;
        }

        public List<Guid> GetAllUsersId()
        {
            string request = @"
                    SELECT id AS UserId 
                    FROM users";

            return _mainConnector.GetList<Guid>(request, null);
        }

        public List<UserDTO> GetAllUsersExpectCurrent(Guid currentUserId)
        {
            string request = @"
                    SELECT
                        id,
                        username,
                        name,
                        surname,
                        patronymic,
                        avatar,
                        current_position AS CurrentPosition
                    FROM
                        users
                    WHERE
                        id != @CurrentUserId";

            var parameters = new DynamicParameters();
            parameters.Add("@CurrentUserId", currentUserId);

            return _mainConnector.GetList<UserDTO>(request, parameters);
        }
    }
}
