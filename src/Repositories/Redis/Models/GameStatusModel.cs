namespace RedisRepository.Models
{
    public class GameStatusModel
    {
        public int HostID { get; set; }
        public int GameID { get; set; }
        public int[] PlayerIDs { get; set; }

    }
}
