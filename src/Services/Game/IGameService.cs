using DAL.Models;
using GameLogic.Game;

namespace Services.Game
{
    public interface IGameService
    {
        IBoardGame LoadGame(GameStatusModel gameStatus);

        GameStatusModel InitGame(GameStatusModel gameStatus);
    }
}
