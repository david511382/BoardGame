using DAL.Interfaces;
using DAL.Structs;
using DAL.Utils;

namespace DAL
{
    public class GameStatusKey : RedisKey<GameStatusModel>, IGameStatusKey
    {
        protected override string KEY => Key.GameStatus;

        public GameStatusKey(IRedisContext redis)
            : base(redis)
        { }

        protected override string GET_HASH_KEY(GameStatusModel value)
        {
            return value.Room.HostID.ToString();
        }
    }
}