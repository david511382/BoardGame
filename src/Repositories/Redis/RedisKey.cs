using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisRepository
{
    public abstract class RedisKey<T> where T : new()
    {
        protected static readonly TimeSpan LOCK_EXPIRY;
        protected static readonly string LOCKER;
        protected static readonly string[] LOCK_KEYS;
        protected static readonly int MASTER_NODES_COUNT;

        static RedisKey()
        {
            LOCK_EXPIRY = TimeSpan.FromSeconds(3);
            LOCKER = Environment.MachineName;
            LOCK_KEYS = new string[] { Key.Lock1, Key.Lock2, Key.Lock3 };
            MASTER_NODES_COUNT = LOCK_KEYS.Length;
        }

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
            return await getLock($"{KEY}{id}");
        }

        virtual public async Task Release(int id)
        {
            await releaseLock($"{KEY}{id}");
        }

        protected async Task<bool> getLock(string key)
        {
            Task<bool>[] getLockTasks = new Task<bool>[MASTER_NODES_COUNT];
            for (int i = 0; i < MASTER_NODES_COUNT; i++)
            {
                string k = $"{LOCK_KEYS[i]}{key}";
                getLockTasks[i] = _db.LockTakeAsync(k, LOCKER, LOCK_EXPIRY);
            }

            int successCount = 0;
            foreach (Task<bool> isSuccess in getLockTasks)
                if (await isSuccess)
                    successCount++;
            if (successCount > Math.Ceiling((double)MASTER_NODES_COUNT / 2))
                return true;

            await releaseLock(key);
            return false;
        }

        protected async Task releaseLock(string key)
        {
            Task<bool>[] releaseLockTasks = new Task<bool>[MASTER_NODES_COUNT];
            for (int i = 0; i < MASTER_NODES_COUNT; i++)
            {
                string k = $"{LOCK_KEYS[i]}{key}";
                releaseLockTasks[i] = _db.LockReleaseAsync(k, LOCKER);
            }

            foreach (Task<bool> isSuccess in releaseLockTasks)
                await isSuccess;
        }
    }
}