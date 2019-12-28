using GameRespository.Models;
using RedisRepository;
using System.Threading.Tasks;

namespace LobbyWebService.Services
{
    public class RedisService : IRedisService
    {
        private GameDAL _gameDAL;
        private RoomDAL _roomDAL;

        public RedisService(string connectStr)
        {
            _gameDAL = new GameDAL(connectStr);
            _roomDAL = new RoomDAL(connectStr);
        }

        public async Task<GameInfo> Game(int ID)
        {
            return await _gameDAL.Game(ID);
        }

        public async Task<GameInfo[]> List()
        {
            return await _gameDAL.List();
        }

        public async Task AddGames(GameInfo[] games)
        {
            await _gameDAL.AddGames(games);
        }

        public async Task CreateRoom(int hostID, GameInfo game)
        {
            await _roomDAL.CreateRoom(hostID, game);
        }
    }
}