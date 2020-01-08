using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisRepository
{
    public partial class RedisDAL
    {
        private ISubscriber _sub => _redis.GetSubscriber();

        public async Task<bool> SetGameStatusNotifyHandler(string key, int hostID)
        {
            return await _db.HashSetAsync(Key.GameStatus + key, hostID.ToString(), "true");
        }

        public async Task<bool> GetGameStatusNotifyHandler(string key, int hostID)
        {
            RedisValue roomValue = await _db.HashGetAsync(Key.GameStatus + key, hostID.ToString());
            if (!string.IsNullOrEmpty(roomValue) && roomValue.Equals("true"))
                return true;
            return false;
        }

        public Task DeleteGameStatusNotifyHandler(string key, int hostID)
        {
            return _db.HashDeleteAsync(Key.GameStatus + key, hostID.ToString());
        }

        public async Task Subscribe(string channel, Action<RedisChannel, RedisValue> handler)
        {
            await _sub.SubscribeAsync(channel, handler);
        }

        public async Task Publish(string channel, string message)
        {
            await _sub.PublishAsync(channel, message);
        }
    }
}