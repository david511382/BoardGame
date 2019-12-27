using GameRespository.Models;

namespace LobbyWebService.Models
{
    public class RedisRoomModel
    {
        public int HostID { get; set; }
        public GameInfo Game { get; set; }
        public int[] PlayerIDs { get; set; }
    }
}
