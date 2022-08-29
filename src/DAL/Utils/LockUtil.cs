using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DAL.Utils
{
    public static class LockUtil
    {
        private static readonly TimeSpan LOCK_EXPIRY;
        private static readonly string LOCKER;
        private static readonly string[] LOCK_KEYS;
        private static readonly int MASTER_NODES_COUNT;

        static LockUtil()
        {
            LOCK_EXPIRY = TimeSpan.FromSeconds(3);
            LOCKER = Environment.MachineName;
            LOCK_KEYS = new string[] { Key.Lock1, Key.Lock2, Key.Lock3 };
            MASTER_NODES_COUNT = LOCK_KEYS.Length;
        }

        public static async Task<bool> GetLock(IDatabaseAsync db, string key)
        {
            Task<bool>[] getLockTasks = new Task<bool>[MASTER_NODES_COUNT];
            for (int i = 0; i < MASTER_NODES_COUNT; i++)
            {
                string k = $"{LOCK_KEYS[i]}{key}";
                getLockTasks[i] = db.LockTakeAsync(k, LOCKER, LOCK_EXPIRY);
            }

            int successCount = 0;
            foreach (Task<bool> isSuccess in getLockTasks)
                if (await isSuccess)
                    successCount++;
            if (successCount > Math.Ceiling((double)MASTER_NODES_COUNT / 2))
                return true;

            await ReleaseLock(db, key);
            return false;
        }

        public static async Task ReleaseLock(IDatabaseAsync db, string key)
        {
            Task<bool>[] releaseLockTasks = new Task<bool>[MASTER_NODES_COUNT];
            for (int i = 0; i < MASTER_NODES_COUNT; i++)
            {
                string k = $"{LOCK_KEYS[i]}{key}";
                releaseLockTasks[i] = db.LockReleaseAsync(k, LOCKER);
            }

            foreach (Task<bool> isSuccess in releaseLockTasks)
                await isSuccess;
        }
    }
}