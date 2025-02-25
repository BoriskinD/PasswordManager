using Microsoft.EntityFrameworkCore;
using Server.Model;

namespace Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        //Представляет таблицу в БД с именем Application
        public DbSet<Application> Application { get; set; }

        //Представляет таблицу в БД с именем Users
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Установка связи "1 ко многим" между таблицами Users и Application
            //Один пользователь может иметь много приложений
            modelBuilder.Entity<User>()
                        .HasMany(user => user.Apps)
                        .WithOne(application => application.User)
                        .HasForeignKey(application => application.UserId)
                        .IsRequired();
        }
    }
}
