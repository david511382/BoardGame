using StackExchange.Redis;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRedisKey<T> where T : new()
    {
        Task<T> Get(int id);

        Task<bool> Set(T value, ITransaction tran = null);

        Task Delete(int id, ITransaction tran = null);

        Task<bool> Lock(int id);

        Task Release(int id);
        
        ITransaction Begin();
    }
}
