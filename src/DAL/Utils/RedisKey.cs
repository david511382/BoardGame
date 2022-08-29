using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace DAL.Utils
{
    public abstract class RedisKey<T> where T : new()
    {
        protected abstract string KEY { get; }

        protected readonly ConnectionMultiplexer _redis;
        protected IDatabase _db => _redis.GetDatabase();

        public RedisKey(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        protected abstract string GET_HASH_KEY(T value);

        virtual public async Task<T> Get(int id)
        {
            RedisValue value = await _db.HashGetAsync(KEY, id.ToString());
            return JsonConvert.DeserializeObject<T>(value);
        }

        virtual public Task<bool> Set(T value, ITransaction tran = null)
        {
            IDatabaseAsync db = (IDatabaseAsync)tran ?? _db;
            string hashKey = GET_HASH_KEY(value);
            return db.HashSetAsync(KEY, hashKey, JsonConvert.SerializeObject(value));
        }

        virtual public Task Delete(int id, ITransaction tran = null)
        {
            IDatabaseAsync db = (IDatabaseAsync)tran ?? _db;
            return db.HashDeleteAsync(KEY, id.ToString());
        }

        virtual public async Task<bool> Lock(int id)
        {
            return await LockUtil.GetLock(_db, $"{KEY}{id}");
        }

        virtual public async Task Release(int id)
        {
            await LockUtil.ReleaseLock(_db, $"{KEY}{id}");
        }
    }
}