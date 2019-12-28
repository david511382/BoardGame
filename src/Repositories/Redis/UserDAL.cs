using Newtonsoft.Json;
using RedisRepository.Models;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisRepository
{
    public class UserDAL
    {
        private ConnectionMultiplexer _redis;
        private IDatabase _db => _redis.GetDatabase();

        public UserDAL(string connectStr)
        {
            _redis = ConnectionMultiplexer.Connect(connectStr);
        }

        public async Task<UserModel> User(int userID)
        {
            RedisValue roomValue = await _db.HashGetAsync(Key.User, userID.ToString());
            return JsonConvert.DeserializeObject<UserModel>(roomValue);
        }

        public async Task AddUser(UserModel user)
        {
            HashEntry[] entry = new HashEntry[] { new HashEntry(user.UserID, JsonConvert.SerializeObject(user)) };
            await _db.HashSetAsync(Key.User, entry);
        }
    }
}