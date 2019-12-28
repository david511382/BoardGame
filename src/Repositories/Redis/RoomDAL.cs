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

        public async Task CreateRoom(int hostID, GameInfo game)
        {
            RedisRoomModel room = new RedisRoomModel();
            room.Game = game;
            room.HostID = hostID;

            HashEntry[] entry = new HashEntry[] { new HashEntry(hostID, JsonConvert.SerializeObject(room)) };
            await _db.HashSetAsync(Key.Room, entry);
        }
    }
}