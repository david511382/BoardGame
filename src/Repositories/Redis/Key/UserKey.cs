using RedisRepository.Models;
using StackExchange.Redis;

namespace RedisRepository
{
    public class UserKey : RedisKey<UserModel>
    {
        protected override string KEY => Key.User;

        public UserKey(ConnectionMultiplexer redis)
            : base(redis)
        { }

        protected override string GET_HASH_KEY(UserModel value)
        {
            return value.UserInfo.ID.ToString();
        }
    }
}