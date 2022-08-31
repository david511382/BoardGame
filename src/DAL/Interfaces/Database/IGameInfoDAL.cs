using DAL.Structs;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IGameInfoDAL
    {
        Task<GameInfo[]> Search(string nameSearchString);
        Task<GameInfo[]> List(int? pageSize = null, int? pageIndex = null);
    }
}
