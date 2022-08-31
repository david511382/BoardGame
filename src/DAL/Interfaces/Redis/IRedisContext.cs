using StackExchange.Redis;

namespace DAL.Interfaces
{
    public interface IRedisContext
    {
        IDatabase GetDatabase(int db = -1, object asyncState = null);
        ISubscriber GetSubscriber(object asyncState = null);
    }
}
