﻿using DAL.Interfaces;
using DAL.Structs;
using DAL.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class GameKey : RedisKey<GameModel>, IGameKey
    {
        protected override string KEY => Key.Game;

        public GameKey(IRedisContext redis)
            : base(redis)
        { }

        public async Task<GameModel[]> ListGames()
        {
            HashEntry[] entrys = await _db.HashGetAllAsync(KEY);
            return entrys.Select((e) =>
            {
                GameModel result = JsonConvert.DeserializeObject<GameModel>(e.Value);

                return result;
            }).ToArray();
        }

        public Task AddGames(GameModel[] games, ITransaction tran = null)
        {
            IDatabaseAsync db = (IDatabaseAsync)tran ?? _db;
            HashEntry[] datas = games.Select((g) => new HashEntry(GET_HASH_KEY(g), JsonConvert.SerializeObject(g)))
                 .ToArray();

            return db.HashSetAsync(KEY, datas);
        }

        protected override string GET_HASH_KEY(GameModel value)
        {
            return value.ID.ToString();
        }
    }
}