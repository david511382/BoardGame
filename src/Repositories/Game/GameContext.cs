
using GameRespository.Models;
using Microsoft.EntityFrameworkCore;

namespace GameRespository
{
    public class GameContext : DbContext
    {
        public DbSet<GameInfo> GameInfos { get; set; }

        public GameContext(DbContextOptions<GameContext> options)
              : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameInfo>()
              .HasIndex(i => new { i.ID, i.Name });
            modelBuilder.Entity<GameInfo>()
                .HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}
