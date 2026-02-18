namespace Client.Model
{
    public class User
    {
        public int Id { get; set; }

        public string? Login { get; set; }

        public string? PasswordHash { get; set; }

        public byte[]? AuthSalt { get; set; }

        public byte[]? EncryptionSalt { get; set; }
    }
}
