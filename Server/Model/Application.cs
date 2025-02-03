using System.ComponentModel.DataAnnotations;

namespace Server.Model
{
    public class Application
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? UserLogin { get; set; }

        [Required]
        public string? UserPassword { get; set; }

        public string? ImagePath { get; set; }

        public int UserId { get; set; }

        public Users User { get; set; } = null!;
    }
}
