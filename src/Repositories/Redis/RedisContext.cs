using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisRepository
{
    public class RedisContext
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

        public async Task<bool> Publish(Channel channel, int identity, string message)
        {
            if (!await ChannelManager.CreateChannelStatus(channel, identity))
                return false;

            await _sub.PublishAsync(channel.ToString(), message);

            return true;
        }
    }
}