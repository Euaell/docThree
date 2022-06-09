using Microsoft.EntityFrameworkCore;


namespace QuizApi.Models
{
    public class QuizDbContext: DbContext
    {
        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
        {
            
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<participant> Participants { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=nerd;user=root;password=1560");
        }
    }
}
