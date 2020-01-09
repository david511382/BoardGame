using RedisRepository.Models;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisRepository
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