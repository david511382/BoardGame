
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DAL
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

            modelBuilder.Entity<GameInfo>().HasData(
                new GameInfo()
                {
                    ID = 1,
                    Name = "軍棋",
                    Description = "象棋遊戲",
                    MaxPlayerCount = 2,
                    MinPlayerCount = 2,
                    CreateTime = DateTime.Now
                },
                new GameInfo()
                {
                    ID = 2,
                    Name = "大老二",
                    Description = "撲克牌遊戲",
                    MaxPlayerCount = 4,
                    MinPlayerCount = 2,
                    CreateTime = DateTime.Now
                }
            );
        }
    }
}
