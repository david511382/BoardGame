using RedisRepository.Models;

namespace GameWebService.Services
{
    public interface IGameService
    {
        GameStatusModel InitGame(GameStatusModel gameStatus);
    }
}
