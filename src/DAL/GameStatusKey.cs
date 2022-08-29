using DAL.Structs;
using DAL.Utils;
using StackExchange.Redis;

namespace DAL
{
    public class GameStatusKey : RedisKey<GameStatusModel>
    {
        protected override string KEY => Key.GameStatus;

        public GameStatusKey(ConnectionMultiplexer redis)
            : base(redis)
        { }

        protected override string GET_HASH_KEY(GameStatusModel value)
        {
            return value.Room.HostID.ToString();
        }
    }
}