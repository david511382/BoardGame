using DAL;
using DAL.Interfaces;
using DAL.Structs;
using DAL.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class GameInfoDAL : IGameInfoDAL
    {
        private GameContext _ctx;

        public GameInfoDAL(GameContext ctx)
        {
            _ctx = ctx;
        }

        ~GameInfoDAL()
        {
            _ctx.Dispose();
        }

        public async Task<GameInfo[]> Search(string nameSearchString)
        {
            if (string.IsNullOrEmpty(nameSearchString))
                throw new Exception("不合法的搜尋條件");

            IQueryable<GameInfo> gamesIQ = from s in _ctx.GameInfos
                                           select s;

            gamesIQ = gamesIQ.Where(s => s.Name.Contains(nameSearchString));

            PaginatedList<GameInfo> games = await PaginatedList<GameInfo>.CreateAsync(gamesIQ.AsNoTracking());

            return games.ToArray();
        }

        public async Task<GameInfo[]> List(int? pageSize = null, int? pageIndex = null)
        {
            IQueryable<GameInfo> gamesIQ = from s in _ctx.GameInfos
                                           select s;

            PaginatedList<GameInfo> games = await PaginatedList<GameInfo>.CreateAsync(
                gamesIQ.AsNoTracking(), pageSize ?? -1, pageIndex ?? 1);

            return games.ToArray();
        }
    }
}
