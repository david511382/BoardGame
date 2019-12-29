using StackExchange.Redis;

namespace RedisRepository
{
    public partial class RedisDAL
    {
        private ConnectionMultiplexer _redis;
        private IDatabase _db => _redis.GetDatabase();

        public RedisDAL(string connectStr)
        {
            _redis = ConnectionMultiplexer.Connect(connectStr);
        }

        public ITransaction Begin()
        {
            return _db.CreateTransaction();
        }
    }
}