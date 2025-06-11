using fishingShopProject.Tools.Extensions;
using Server.Models.User;
using Server.Repository.Interface;
using Server.Services.Interface;
using Server.Tools;
using System.Security.Cryptography;
using System.Text;

namespace Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Result CreateUser(
            string username,
            string password,
                   string name,
                   string surname,
                   string? patronymic,
                   string currentPosition,
                   byte[]? avatar)
        {
            try
            {
                var user = new UserDB
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Password = password,
                    Name = name,
                    Surname = surname,
                    Patronymic = patronymic,
                    CurrentPosition = currentPosition,
                    Avatar = avatar,
                    Role = "Сотрудник"
                };

                _userRepository.CreateUser(user);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при создании пользователя: {ex.Message}");
            }
        }

        public Result UpdateUser(
            Guid userid,
                  string name,
                  string surname,
                  string? patronymic,
                  string currentPosition,
                  byte[]? avatar)
        {
            try
            {
                var user = new UserDB
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Surname = surname,
                    Patronymic = patronymic,
                    CurrentPosition = currentPosition,
                    Avatar = avatar
                };

                _userRepository.UpdateUser(user);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при создании пользователя: {ex.Message}");
            }
        }

        public Result<UserDB> Login(string? login, string? password)
        {
            if (login.IsNullOrWhiteSpace()) return Result<UserDB>.Fail("Укажите логин");

            if (password.IsNullOrWhiteSpace()) return Result<UserDB>.Fail("Укажите пароль");

            string passwordHash = HashPassword(password!);
            var user = _userRepository.GetUser(login!, passwordHash!);

            return user == null
                ? Result<UserDB>.Fail("Неправильный логин или пароль")
                : Result<UserDB>.Success(user);
        }

        public Result<List<Guid>> GetAllUsersId()
        {
            var users = _userRepository.GetAllUsersId();

            return users == null
                ? Result<List<Guid>>.Fail("Чаты не найдены")
                : Result<List<Guid>>.Success(users);
        }

        public Result<List<UserDTO>> GetAllUsersExpectCurrent(Guid currentUserId)
        {
            var users = _userRepository.GetAllUsersExpectCurrent(currentUserId);

            return users == null
                ? Result<List<UserDTO>>.Fail("Чаты не найдены")
                : Result<List<UserDTO>>.Success(users);
        }

        private string HashPassword(string password)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(hash);
        }
    }
}
