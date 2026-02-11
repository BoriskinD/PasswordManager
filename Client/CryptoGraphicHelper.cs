using System.Security.Cryptography;
using System.Text;

namespace Client
{
    public class CryptoGraphicHelper
    {
        public byte[] GenerateSalt(int size = 32)
        {
            byte[] salt;
            return salt = RandomNumberGenerator.GetBytes(size);
        }

        public string HashPassword(string password, byte[] salt)
        { 
            string passwordHash = Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(
                                                         Encoding.UTF8.GetBytes(password),
                                                         salt,
                                                         100000,
                                                         HashAlgorithmName.SHA256,
                                                         outputLength: 32)); //32 байта
            return passwordHash;
        }
    }
}
