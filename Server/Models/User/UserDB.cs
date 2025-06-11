namespace Server.Models.User
{
    public class UserDB
    {
        public Guid Id { get; set; }
        public byte[]? Avatar { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string CurrentPosition { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
