using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DAL.Utils
{
    public class ChannelStatusManager
    {
        public enum WorkChannel
        {
            WaitingForHandle,
            Handling,
            Done
        }

        private static readonly TimeSpan KEY_EXPIRY = TimeSpan.FromMinutes(1);

        private string KEY => Key.Channel;
        private readonly ConnectionMultiplexer _redis;
        private IDatabase _db => _redis.GetDatabase();

        public ChannelStatusManager(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<bool> CreateChannelStatus(Channel channel, int identity)
        {
            string hashKey = identity.ToString();
            RedisKey key = $"{KEY}{channel.ToString()}{identity}";

            bool success = await _db.HashSetAsync(key, hashKey, WorkChannel.WaitingForHandle.ToString());
            if (success)
                await _db.KeyExpireAsync(key, KEY_EXPIRY);

            return success;
        }

        public async Task<bool> IsWaitingForHandle(Channel channel, int identity)
        {
            string hashKey = identity.ToString();
            RedisKey key = $"{KEY}{channel.ToString()}{identity}";

            RedisValue value = await _db.HashGetAsync(key, hashKey);
            WorkChannel status = (WorkChannel)Enum.Parse(typeof(WorkChannel), value);
            if (status == WorkChannel.WaitingForHandle)
                return true;
            return false;
        }

        public async Task<bool> Handle(Channel channel, int identity)
        {
            string hashKey = identity.ToString();
            RedisKey key = $"{KEY}{channel.ToString()}{identity}";

            if (!await LockUtil.GetLock(_db, key))
                return false;

            bool result = true;
            if (await _db.KeyExistsAsync(key))
            {
                result = await _db.HashSetAsync(key, hashKey, WorkChannel.Handling.ToString());
                await _db.KeyExpireAsync(key, KEY_EXPIRY);
            }

            return result;
        }

        public async Task<bool> Done(Channel channel, int identity)
        {
            string hashKey = identity.ToString();
            RedisKey key = $"{KEY}{channel.ToString()}{identity}";

            bool result = true;
            if (await _db.KeyExistsAsync(key))
            {
                result = await _db.HashSetAsync(key, hashKey, WorkChannel.Done.ToString());
                await _db.KeyExpireAsync(key, KEY_EXPIRY);
            }

            await LockUtil.ReleaseLock(_db, $"{KEY}{hashKey}");

            return result;
        }
    }
}