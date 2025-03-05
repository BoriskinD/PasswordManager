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

        [Required]
        public int UserId { get; set; }

        public string? ImagePath { get; set; }

        //Это свойство используетсся только для связи таблиц
        //Является опциональным во избежание ошибок валидации т.к.
        //класс Application (MyApp) не содержит свойство User на клиенте
        public User? User { get; set; } 
    }
}
