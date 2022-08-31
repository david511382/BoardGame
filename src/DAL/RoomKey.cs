using DAL.Interfaces;
using DAL.Structs;
using DAL.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class RoomKey : RedisKey<RoomModel>, IRoomKey
    {
        protected override string KEY => Key.Room;

        public RoomKey(IRedisContext redis)
            : base(redis)
        { }

        public async Task<RoomModel[]> ListRooms()
        {
            HashEntry[] entrys = await _db.HashGetAllAsync(KEY);
            return entrys.Select((e) =>
            {
                RoomModel result = JsonConvert.DeserializeObject<RoomModel>(e.Value);

                return result;
            }).ToArray();
        }

        protected override string GET_HASH_KEY(RoomModel value)
        {
            return value.HostID.ToString();
        }
    }
}