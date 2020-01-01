using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisRepository
{
    public partial class RedisDAL
    {
        private static readonly TimeSpan LOCK_EXPIRY;
        private static readonly string LOCKER;
        private static readonly string[] LOCK_KEYS;
        private static readonly int MASTER_NODES_COUNT;

        static RedisDAL()
        {
            LOCK_EXPIRY = TimeSpan.FromSeconds(3);
            LOCKER = Environment.MachineName;
            LOCK_KEYS = new string[] { Key.Lock1, Key.Lock2, Key.Lock3 };
            MASTER_NODES_COUNT = LOCK_KEYS.Length;
        }

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

        private async Task<bool> getLock(string key)
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

        private async Task releaseLock(string key)
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