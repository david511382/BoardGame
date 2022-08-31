using DAL.Structs;
using GameLogic.Game;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IGameService
    {
        Task<GameModel> Game(int ID);
        Task<IEnumerable<GameModel>> GameList();

        IBoardGame LoadGame(GameStatusModel gameStatus);

        GameStatus InitGame(GameStatusModel gameStatus);
    }
}
