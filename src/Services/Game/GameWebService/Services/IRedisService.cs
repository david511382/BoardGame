using RedisRepository.Models;
using System.Threading.Tasks;

namespace GameWebService.Services
{
    public interface IRedisService
    {
        Task<GameStatusModel> GameStatus(int hostID);

        Task SubscribeInitGame();
    }
}
