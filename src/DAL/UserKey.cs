using DAL.Structs;
using DAL.Utils;
using StackExchange.Redis;

namespace DAL
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