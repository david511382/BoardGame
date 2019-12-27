using GameRespository.Models;
using RedisRepository.Services;
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
                GameInfo[] games = await _redisService.List();
                if (games.Length != 0)
                    return;

                games = await _gameService.List();
                await _redisService.AddGames(games);
            });
        }
    }
}
