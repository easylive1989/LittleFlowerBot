using LittleFlowerBot.Models.GameResult;
using Microsoft.EntityFrameworkCore;

namespace LittleFlowerBot.DbContexts
{
    public class LittleFlowerBotContext: DbContext
    {
        public LittleFlowerBotContext(DbContextOptions<LittleFlowerBotContext> options)
            : base(options)
        {
        }

        public DbSet<BoardGameResult> BoardGameGameResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
