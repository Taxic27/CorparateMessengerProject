namespace Server.Models.User
{
    public partial class UserBlank
    {
        public Guid? Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public partial class UserBlank
    {
        public class Validated
        {
            public Guid Id { get;}
            public string Username { get;}
            public string Password { get;}

            public Validated(Guid id, string username, string passwordHash)
            {
                Id = id;
                Username = username;
                Password = passwordHash;
            }
        }
    }
}
