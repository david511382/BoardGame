using GameLogic.Game;
using RedisRepository.Models;

namespace GameWebService.Services
{
    public interface IGameService
    {
        BoardGame LoadGame(GameStatusModel gameStatus);

        GameStatusModel InitGame(GameStatusModel gameStatus);
    }
}
