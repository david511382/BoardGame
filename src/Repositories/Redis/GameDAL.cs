using Newtonsoft.Json;
using RedisRepository.Models;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace RedisRepository
{
    public partial class RedisDAL
    {
        public async Task<GameModel> Game(int ID)
        {
            RedisValue entry = await _db.HashGetAsync(Key.Game, ID.ToString());
            return JsonConvert.DeserializeObject<GameModel>(entry);
        }

        public async Task<GameModel[]> ListGames()
        {
            HashEntry[] entrys = await _db.HashGetAllAsync(Key.Game);
            return entrys.Select((e) =>
             {
                 GameModel result = JsonConvert.DeserializeObject<GameModel>(e.Value);

                 return result;
             }).ToArray();
        }

        public Task AddGames(GameModel[] games, ITransaction tran = null)
        {
            IDatabaseAsync db = (IDatabaseAsync)tran ?? _db;
            HashEntry[] datas = games.Select((g) => new HashEntry(g.ID, JsonConvert.SerializeObject(g)))
                 .ToArray();

            return db.HashSetAsync(Key.Game, datas);
        }
    }
}