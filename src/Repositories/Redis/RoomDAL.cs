using GameRespository.Models;
using Newtonsoft.Json;
using RedisRepository.Models;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisRepository
{
    public class RoomDAL
    {
        private ConnectionMultiplexer _redis;
        private IDatabase _db => _redis.GetDatabase();

        public RoomDAL(string connectStr)
        {
            _redis = ConnectionMultiplexer.Connect(connectStr);
        }

        public async Task<RedisRoomModel> Room(int hostID)
        {
            RedisValue roomValue = await _db.HashGetAsync(Key.Room, hostID.ToString());
            return JsonConvert.DeserializeObject<RedisRoomModel>(roomValue);
        }

        public async Task SetRoom(RedisRoomModel room)
        {
            HashEntry[] entry = new HashEntry[] { new HashEntry(room.HostID, JsonConvert.SerializeObject(room)) };
            await _db.HashSetAsync(Key.Room, entry);
        }
    }
}