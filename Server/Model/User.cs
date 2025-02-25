namespace Server.Model
{
    public class User
    {
        public int Id { get; set; }

        public string? Login { get; set; }

        public string? Password { get; set; }

        public List<Application> Apps { get; set; } = new List<Application>();
    }
}
