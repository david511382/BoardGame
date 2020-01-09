using RedisRepository.Models;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace RedisRepository
{
    public class GameStatusKey : RedisKey<GameStatusModel>
    {
        protected override string KEY => Key.GameStatus;

        private const string GAME_STATUS_NOTIFY_HANDLER_VALUE = "NotifyStatus";

        private string GAME_STATUS_NOTIFY_HANDLER_KEY => KEY + GAME_STATUS_NOTIFY_HANDLER_VALUE;

        public GameStatusKey(ConnectionMultiplexer redis)
            : base(redis)
        { }

        protected override string GET_HASH_KEY(GameStatusModel value)
        {
            return value.Room.HostID.ToString();
        }

        public async Task<bool> SetGameStatusNotifyHandler(int id)
        {
            string key = id.ToString();
            return await _db.HashSetAsync(GAME_STATUS_NOTIFY_HANDLER_KEY + key, key, GAME_STATUS_NOTIFY_HANDLER_VALUE);
        }

        public async Task<bool> GetGameStatusNotifyHandler(int id)
        {
            string key = id.ToString();
            RedisValue roomValue = await _db.HashGetAsync(GAME_STATUS_NOTIFY_HANDLER_KEY + key, key);
            if (!string.IsNullOrEmpty(roomValue) && roomValue.Equals(GAME_STATUS_NOTIFY_HANDLER_VALUE))
                return true;
            return false;
        }

        public Task DeleteGameStatusNotifyHandler(string key, int hostID)
        {
            return _db.HashDeleteAsync(GAME_STATUS_NOTIFY_HANDLER_KEY + key, hostID.ToString());
        }
    }
}