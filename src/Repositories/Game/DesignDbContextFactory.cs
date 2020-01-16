using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace GameRespository
{
    internal class DesignDbContextFactory : IDesignTimeDbContextFactory<GameContext>
    {
        public GameContext CreateDbContext(params string[] args)
        {
            string connectionString = args[0];

            DbContextOptionsBuilder<GameContext> builder = new DbContextOptionsBuilder<GameContext>();
            builder.UseSqlServer(connectionString);
            return new GameContext(builder.Options);
        }
    }
}
