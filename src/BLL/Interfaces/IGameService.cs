using DAL.Structs;
using GameLogic.Game;

namespace BLL.Interfaces
{
    public interface IGameService
    {
        IBoardGame LoadGame(GameStatusModel gameStatus);

        GameStatusModel InitGame(GameStatusModel gameStatus);
    }
}
