using GameRespository.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace RedisRepository
{
    public class GameDAL
    {
        private ConnectionMultiplexer _redis;
        private IDatabase _db => _redis.GetDatabase();

        public GameDAL(string connectStr)
        {
            _redis = ConnectionMultiplexer.Connect(connectStr);
        }

        public async Task<GameInfo> Game(int ID)
        {
            RedisValue entry = await _db.HashGetAsync(Key.Game, ID.ToString());
            return JsonConvert.DeserializeObject<GameInfo>(entry);
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

        public async Task AddGames(GameInfo[] games)
        {
            HashEntry[] datas = games.Select((g) => new HashEntry(g.ID, JsonConvert.SerializeObject(g)))
                 .ToArray();

            await _db.HashSetAsync(Key.Game, datas);
        }
    }
}