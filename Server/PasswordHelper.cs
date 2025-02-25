using System.Security.Cryptography;
using System.Text;

namespace Server
{
    public class PasswordHelper
    {
        public static string CreateHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword) 
        {
            string hash = CreateHash(password);
            return hash == hashedPassword;
        }
    }
}
