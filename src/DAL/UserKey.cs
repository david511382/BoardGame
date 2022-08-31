using DAL.Interfaces;
using DAL.Structs;
using DAL.Utils;

namespace DAL
{
    public class UserKey : RedisKey<UserModel>, IUserKey
    {
        protected override string KEY => Key.User;

        public UserKey(IRedisContext redis)
            : base(redis)
        { }

        protected override string GET_HASH_KEY(UserModel value)
        {
            return value.UserInfo.ID.ToString();
        }
    }
}