using DAL.Interfaces;
using DAL.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DAL
{
    public class RedisContext : IRedisContext, IDisposable
    {
        public ChannelStatusManager ChannelManager;

        protected ConnectionMultiplexer _redis;
        private ISubscriber _sub => _redis.GetSubscriber();

        public RedisContext(string connectStr)
        {
            _redis = ConnectionMultiplexer.Connect(connectStr);
            ChannelManager = new ChannelStatusManager(_redis);
        }

        public IDatabase GetDatabase(int db = -1, object asyncState = null) => _redis.GetDatabase(db, asyncState);
        public ISubscriber GetSubscriber(object asyncState = null) => _redis.GetSubscriber(asyncState);

        public async Task Subscribe(Channel channel, Action<RedisChannel, RedisValue> handler)
        {
            await _sub.SubscribeAsync(channel.ToString(), handler);
        }

        public async Task<bool> Publish<T>(Channel channel, T json, int identity = 0)
        {
            if (identity != 0)
                if (!await ChannelManager.CreateChannelStatus(channel, identity))
                    return false;

            string msg = JsonConvert.SerializeObject(json);
            await _sub.PublishAsync(channel.ToString(), msg);

            return true;
        }

        public void Dispose()
        {
            _redis.Close();
        }
    }
}