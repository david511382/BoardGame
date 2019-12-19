using GameRespository.Models;
using Newtonsoft.Json;
using RedisRepository;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace LobbyWebService.Sevices
{
    public class RedisService : IRedisService
    {
        private ConnectionMultiplexer _redis;
        private IDatabase _db => _redis.GetDatabase();

        public RedisService(string connectStr)
        {
            _redis = ConnectionMultiplexer.Connect(connectStr);
        }

        public async Task<GameInfo[]> List()
        {
            HashEntry[] entrys = await _db.HashGetAllAsync(Key.Game);
            return entrys.Select((e) =>
             {
                 GameInfo result = JsonConvert.DeserializeObject<GameInfo>(e.Value);

                 return result;
             }).ToArray();
        }
    }
}