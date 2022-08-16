using GameRespository.Models;
using System.Threading.Tasks;

namespace GameWebService.Services
{
    public interface IGameInfoDAL
    {
        Task<GameInfo[]> Search(string nameSearchString);
        Task<GameInfo[]> List(int? pageSize = null, int? pageIndex = null);
    }
}
