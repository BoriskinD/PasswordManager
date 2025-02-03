namespace Client.Model
{
    public class User
    {
        public int Id { get; set; }

        public string? Login { get; set; }

        public string? PasswordHash { get; set; }

        public List<MyApp>? Apps { get; set; }
    }
}
