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
            return db.HashSetAsync(Key.User, user.UserInfo.ID, JsonConvert.SerializeObject(user));
        }

        public async Task<bool> LockUser(int userID)
        {
            return await getLock($"{Key.User}{userID}");
        }

        public async Task ReleaseUser(int userID)
        {
            await releaseLock($"{Key.User}{userID}");
        }
    }
}