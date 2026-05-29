namespace Client
{
    //Класс для хранения мастер ключа для сессии текущего пользователя
    class SecureSession 
    {
        private static SecureSession? instance;
        private static object lockObj = new object();
        private byte[]? masterKey;

        private SecureSession() {}

        public static SecureSession getInstance()
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    instance = new SecureSession();
                }
            }
            return instance;
        }

        public void Initialize(string masterPassword, byte[] encryptionSalt)
        { 
            Clear();
            masterKey = CryptoGraphicHelper.CreateKey(masterPassword, encryptionSalt);
        }

        public string Decrypt(byte[] data)
        {
            return CryptoGraphicHelper.Decrypt(data, masterKey);
        }

        public byte[] Encrypt(string data)
        {
            return CryptoGraphicHelper.Encrypt(data, masterKey);
        }

        public void Clear()
        {
            if (masterKey != null)
            {
                Array.Clear(masterKey);
                masterKey = null;
            }
        }
    }
}
