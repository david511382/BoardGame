using GameLogic.Game;
using RedisRepository.Models;

namespace Services.Game
{
    public interface IGameService
    {
        IBoardGame LoadGame(GameStatusModel gameStatus);

        GameStatusModel InitGame(GameStatusModel gameStatus);
    }
}
