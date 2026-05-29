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
                                                         600_000,
                                                         HashAlgorithmName.SHA256,
                                                         32)); //32 байта
            return passwordHash;
        }

        public static byte[] CreateKey(string masterPassword, byte[] encryptionSalt) 
        {
            return Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(masterPassword), encryptionSalt,
                                             600_000, HashAlgorithmName.SHA256, 32);
        }

        public static byte[] Encrypt(string password, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(password);
            byte[] encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            byte[] result = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);
            return result;
        }

        public static string Decrypt(byte[] encryptedData, byte[] key)
        { 
            using Aes aes = Aes.Create();
            aes.Key = key;

            int ivSize = aes.IV.Length;
            byte[] iv = new byte[ivSize];
            byte[] cipherText = new byte[encryptedData.Length - ivSize];
            Buffer.BlockCopy(encryptedData, 0, iv, 0, ivSize);
            Buffer.BlockCopy(encryptedData, ivSize, cipherText, 0, cipherText.Length);

            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv);
            byte[] decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
