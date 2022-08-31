using DAL.Structs;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IGameKey : IRedisKey<GameModel>
    {
        Task<GameModel[]> ListGames();
        Task AddGames(GameModel[] games, ITransaction tran = null);
    }
}
