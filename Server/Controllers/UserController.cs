using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Server.Models.Chat;
using Server.Models.User;
using Server.Services;
using Server.Services.Interface;
using Server.Tools;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public record LoginRequest(string Login, string Password);

        [HttpPost("login")]
        public Result<UserDB> Login([FromBody] LoginRequest request)
        {
            var result = _userService.Login(request.Login, request.Password);

            if (!result.IsSuccess)
                return Result<UserDB>.Fail(result.ErrorsAsString);

            return Result<UserDB>.Success(result.Data!);
        }

        public record CreateUserRequest(
            string Username,
            string Password,
           string Name,
           string Surname,
           string? Patronymic,
           string CurrentPosition,
           byte[]? Avatar);

        [HttpPost("create")]
        public Result CreateUser([FromBody] CreateUserRequest request)
        {
            var result = _userService.CreateUser(
                request.Username,
                request.Password,
                request.Name,
                request.Surname,
                request.Patronymic,
                request.CurrentPosition,
                request.Avatar);

            return result.IsSuccess
                ? Result.Success()
                : Result.Fail(result.ErrorsAsString);
        }

        public record UpdateUserRequest(
            Guid UserId,
           string Name,
           string Surname,
           string? Patronymic,
           string CurrentPosition,
           byte[]? Avatar);

        [HttpPut("update")]
        public Result UpdateUser([FromBody] UpdateUserRequest request)
        {
            var result = _userService.UpdateUser(
                request.UserId,
                request.Name,
                request.Surname,
                request.Patronymic,
                request.CurrentPosition,
                request.Avatar);

            return result.IsSuccess
                ? Result.Success()
                : Result.Fail(result.ErrorsAsString);
        }

        [HttpGet("id")]
        public Result<List<Guid>> GetAllUsersId()
        {
            var result = _userService.GetAllUsersId();
            return result.IsSuccess
                ? Result<List<Guid>>.Success(result.Data!)
                : Result<List<Guid>>.Fail(result.ErrorsAsString);
        }

        [HttpGet("get-users")]
        public Result<List<UserDTO>> GetAllUsersExpectCurrent(Guid currentUserId)
        {
            var result = _userService.GetAllUsersExpectCurrent(currentUserId);
            return result.IsSuccess
                ? Result<List<UserDTO>>.Success(result.Data!)
                : Result<List<UserDTO>>.Fail(result.ErrorsAsString);
        }
    }
}
