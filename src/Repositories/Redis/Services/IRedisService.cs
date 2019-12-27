using GameRespository.Models;
using System.Threading.Tasks;

namespace RedisRepository.Services
{
    public interface IRedisService
    {
        Task<GameInfo[]> List();
        Task<GameInfo> Game(int ID);
        Task AddGames(GameInfo[] games);

        Task CreateRoom(int hostID, GameInfo game);
    }
}
