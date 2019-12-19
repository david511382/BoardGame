using GameRespository;
using GameRespository.Models;
using System.Threading.Tasks;

namespace LobbyWebService.Sevices
{
    class GameService : IGameService
    {
        private GameInfoDAL _db;

        public GameService(string dbConnectStr)
        {
            _db = new GameInfoDAL(dbConnectStr);
        }

        public async Task<GameInfo[]> List()
        {
            return await _db.List();
        }
    }
}
