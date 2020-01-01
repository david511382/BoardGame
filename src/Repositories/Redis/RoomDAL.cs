using Domain.Api.Models.Base.Lobby;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace RedisRepository
{
    public partial class RedisDAL
    {
        public async Task<RoomModel[]> ListRooms()
        {
            HashEntry[] entrys = await _db.HashGetAllAsync(Key.Room);
            return entrys.Select((e) =>
            {
                RoomModel result = JsonConvert.DeserializeObject<RoomModel>(e.Value);

                return result;
            }).ToArray();
        }

        public async Task<RoomModel> Room(int hostID)
        {
            RedisValue roomValue = await _db.HashGetAsync(Key.Room, hostID.ToString());
            return JsonConvert.DeserializeObject<RoomModel>(roomValue);
        }

        public Task SetRoom(RoomModel room, ITransaction tran = null)
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