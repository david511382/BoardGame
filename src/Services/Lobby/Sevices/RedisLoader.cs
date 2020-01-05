using LobbyWebService.Services;
using RedisRepository.Models;
using System.Linq;
using System.Threading.Tasks;

namespace LobbyWebService.Sevices
{
    class RedisLoader
    {
        IRedisService _redisService;
        IGameService _gameService;

        public RedisLoader(IRedisService redisService, IGameService gameService)
        {
            _redisService = redisService;
            _gameService = gameService;
        }

        public Task Load()
        {
            return Task.Run(async () =>
            {
                GameModel[] games = await _redisService.ListGames();
                if (games.Length != 0)
                    return;

                games = (await _gameService.List())
                    .Select((g) => new GameModel
                    {
                        ID = g.ID,
                        Description = g.Description,
                        MaxPlayerCount = g.MaxPlayerCount,
                        MinPlayerCount = g.MinPlayerCount,
                        Name = g.Name
                    })
                    .ToArray();
                await _redisService.AddGames(games);
            });
        }
    }
}
