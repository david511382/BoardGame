using GameRespository.Models;

namespace RedisRepository.Models
{
    public class RedisRoomModel
    {
        public int HostID { get; set; }
        public GameInfo Game { get; set; }
        public int[] PlayerIDs { get; set; }

        public bool IsFull()
        {
            return PlayerIDs.Length == Game.MaxPlayerCount;
        }
    }
}
