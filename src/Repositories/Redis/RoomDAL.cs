using Newtonsoft.Json;
using RedisRepository.Models;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisRepository
{
    public partial class RedisDAL
    {
        public async Task<RedisRoomModel> Room(int hostID)
        {
            RedisValue roomValue = await _db.HashGetAsync(Key.Room, hostID.ToString());
            return JsonConvert.DeserializeObject<RedisRoomModel>(roomValue);
        }

        public Task SetRoom(RedisRoomModel room, ITransaction tran = null)
        {
            IDatabaseAsync db = (IDatabaseAsync)tran ?? _db;
            return db.HashSetAsync(Key.Room, room.HostID, JsonConvert.SerializeObject(room));
        }

        public Task DeleteRoom(int roomID, ITransaction tran = null)
        {
            IDatabaseAsync db = (IDatabaseAsync)tran ?? _db;
            return db.HashDeleteAsync(Key.Room, roomID.ToString());
        }

        public async Task<bool> LockRoom(int roomID)
        {
            return await getLock($"{Key.Room}{roomID}");
        }

        public async Task ReleaseRoom(int roomID)
        {
            await releaseLock($"{Key.Room}{roomID}");
        }
    }
}