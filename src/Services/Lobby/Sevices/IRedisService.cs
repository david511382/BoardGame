using GameRespository.Models;
using System.Threading.Tasks;

namespace LobbyWebService.Sevices
{
    public interface IRedisService : IGameService
    {
        Task AddGames(GameInfo[] games);
    }
}
