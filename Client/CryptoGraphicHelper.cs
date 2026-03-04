using System.Security.Cryptography;
using System.Text;

namespace Client
{
    public static class CryptoGraphicHelper
    {
        public static byte[] GenerateSalt(int size = 32)
        {
            return RandomNumberGenerator.GetBytes(size);
        }

        public static string HashPassword(string password, byte[] salt)
        { 
            string passwordHash = Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(
                                                         Encoding.UTF8.GetBytes(password),
                                                         salt,
                                                         100000,
                                                         HashAlgorithmName.SHA256,
                                                         outputLength: 32)); //32 байта
            return passwordHash;
        }

        public static byte[] CreateEncryptionKey(string masterPassword, byte[] encryptionSalt) 
        {
            return Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(masterPassword), encryptionSalt,
                                             100000, HashAlgorithmName.SHA256, 32);
        }

        public static byte[] Encrypt(string password, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] encrypted = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(password),
                                                             0, password.Length);

            byte[] result = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);
            return result;
        }

        public static string Decrypt(byte[] encryptedData, byte[] key)
        { 
            using Aes aes = Aes.Create();
            aes.Key = key;

            byte[] iv = new byte[16];
            byte[] cipherText = new byte[encryptedData.Length - 16];
            Buffer.BlockCopy(encryptedData, 0, iv, 0, 16);
            Buffer.BlockCopy(encryptedData, 16, cipherText, 0, cipherText.Length);

            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);
            byte[] decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
