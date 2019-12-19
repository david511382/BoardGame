using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace GameRespository
{
    internal class DesignDbContextFactory : IDesignTimeDbContextFactory<GameContext>
    {
        public GameContext CreateDbContext(params string[] args)
        {

            string connectionString = "Server=localhost,1488;Initial Catalog=Game;Persist Security Info=True;User ID=LobbyWebService;Password=lobby$WebService;TrustServerCertificate=False;";

            DbContextOptionsBuilder<GameContext> builder = new DbContextOptionsBuilder<GameContext>();
            builder.UseSqlServer(connectionString);
            return new GameContext(builder.Options);
        }
    }
}
