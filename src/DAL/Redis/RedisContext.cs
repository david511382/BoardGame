using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DAL
{
    public class RedisContext : IDisposable
    {
        public UserKey User;
        public RoomKey Room;
        public GameStatusKey GameStatus;
        public GameKey Game;
        public ChannelStatusManager ChannelManager;

        protected ConnectionMultiplexer _redis;
        private ISubscriber _sub => _redis.GetSubscriber();

        public RedisContext(string connectStr)
        {
            _redis = ConnectionMultiplexer.Connect(connectStr);

            User = new UserKey(_redis);
            Room = new RoomKey(_redis);
            GameStatus = new GameStatusKey(_redis);
            Game = new GameKey(_redis);
            ChannelManager = new ChannelStatusManager(_redis);
        }

        public ITransaction Begin()
        {
            return _redis.GetDatabase().CreateTransaction();
        }

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