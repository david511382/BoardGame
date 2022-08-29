using DAL.Models;
using System.Threading.Tasks;

namespace Services.Game
{
    public interface IGameInfoDAL
    {
        Task<GameInfo[]> Search(string nameSearchString);
        Task<GameInfo[]> List(int? pageSize = null, int? pageIndex = null);
    }
}
