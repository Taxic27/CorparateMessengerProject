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

        public void SaveUser(UserBlank.Validated validatedUser)
        {
            string request = @"
                    INSERT INTO users (id, username, password, createdatetime) 
                    VALUES (@Id, @Login, @Password, @CreateDateTime)
                    ON CONFLICT (id) 
                    DO UPDATE SET
                    login = EXCLUDED.login,
                    password = EXCLUDED.password";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", validatedUser.Id);
            parameters.Add("@Username", validatedUser.Username);
            parameters.Add("@Password", validatedUser.Password);
            parameters.Add("@CreateDateTime", DateTime.UtcNow);

            _mainConnector.Execute(request, parameters);
        }

        public UserDB GetUserLogin(string login)
        {
            string request = @"SELECT * FROM users WHERE username = @Login";

            var parameters = new DynamicParameters();
            parameters.Add("@Login", login);

            return _mainConnector.Get<UserDB>(request, parameters);
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
