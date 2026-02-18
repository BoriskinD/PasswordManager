namespace Client
{
    public class ServerResponse
    {
        public int UserId { get; set; }

        public string? Token { get; set; }

        public byte[] AuthSalt { get; set; }

        public byte[] EncryptionSalt { get; set; }
    }
}
