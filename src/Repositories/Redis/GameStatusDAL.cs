using Newtonsoft.Json;
using RedisRepository.Models;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisRepository
{
    public partial class RedisDAL
    {
        public async Task<GameStatusModel> GameStatus(int hostID)
        {
            RedisValue roomValue = await _db.HashGetAsync(Key.GameStatus, (-hostID).ToString());
            return JsonConvert.DeserializeObject<GameStatusModel>(roomValue);
        }

        public Task<bool> SetGameStatus(GameStatusModel gameStatus, ITransaction tran = null)
        {
            IDatabaseAsync db = (IDatabaseAsync)tran ?? _db;
            return db.HashSetAsync(Key.GameStatus, -gameStatus.HostID, JsonConvert.SerializeObject(gameStatus));
        }

        public Task DeleteGameStatus(int hostID, ITransaction tran = null)
        {
            IDatabaseAsync db = (IDatabaseAsync)tran ?? _db;
            return db.HashDeleteAsync(Key.GameStatus, (-hostID).ToString());
        }

        public async Task<bool> LockGameStatus(int hostID)
        {
            return await getLock($"{Key.GameStatus}{hostID}");
        }

        public async Task ReleaseGameStatus(int hostID)
        {
            await releaseLock($"{Key.GameStatus}{hostID}");
        }
    }
}