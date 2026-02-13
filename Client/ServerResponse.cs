namespace Client
{
    public class ServerResponse
    {
        public int UserId { get; set; }
        public string? Token { get; set; }
        public byte[] Salt { get; set; }
    }
}
