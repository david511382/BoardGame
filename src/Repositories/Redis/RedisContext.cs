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

        protected ConnectionMultiplexer _redis;
        private ISubscriber _sub => _redis.GetSubscriber();

        public RedisContext(string connectStr)
        {
            _redis = ConnectionMultiplexer.Connect(connectStr);

            User = new UserKey(_redis);
            Room = new RoomKey(_redis);
            GameStatus = new GameStatusKey(_redis);
            Game = new GameKey(_redis);
        }

        public ITransaction Begin()
        {
            return _redis.GetDatabase().CreateTransaction();
        }

        public async Task Subscribe(Channel channel, Action<RedisChannel, RedisValue> handler)
        {
            await _sub.SubscribeAsync(channel.ToString(), handler);
        }

        public async Task Publish(Channel channel, string message)
        {
            await _sub.PublishAsync(channel.ToString(), message);
        }
    }
}