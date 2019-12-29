using Newtonsoft.Json;
using RedisRepository.Models;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisRepository
{
    public partial class RedisDAL
    {
        public async Task<UserModel> User(int userID)
        {
            RedisValue roomValue = await _db.HashGetAsync(Key.User, userID.ToString());
            return JsonConvert.DeserializeObject<UserModel>(roomValue);
        }

        public Task SetUser(UserModel user, ITransaction tran = null)
        {
            IDatabaseAsync db = (IDatabaseAsync)tran ?? _db;
            return db.HashSetAsync(Key.User, user.UserID, JsonConvert.SerializeObject(user));
        }
    }
}