using GameRespository.Models;
using System.Threading.Tasks;

namespace LobbyWebService.Sevices
{
    public interface IRedisService : IGameService
    {
        Task<GameInfo> Game(int ID);
        Task AddGames(GameInfo[] games);

        Task CreateRoom(int hostID, GameInfo game);
    }
}
