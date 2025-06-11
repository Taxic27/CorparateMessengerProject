using System.Security.Cryptography;
using System.Text;

namespace Server.Tools
{
    public class Password
    {
        public String Value { get; }
        public String Hash => GetHashString(Value);

        public Password(String value)
        {
            Value = value;
        }

        private String GetHashString(String str)
        {
            Byte[] bytes = Encoding.Unicode.GetBytes(str);
            MD5 md5 = MD5.Create();

            Byte[] byteHash = md5.ComputeHash(bytes);
            String hash = Convert.ToHexString(byteHash);

            return hash;
        }
    }
}
