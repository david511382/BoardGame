namespace DAL.Models
{
    public class RoomModel
    {
        public int HostID { get; set; }
        public GameModel Game { get; set; }
        public UserInfoModel[] Players { get; set; }

        public bool IsFull()
        {
            return Players.Length == Game.MaxPlayerCount;
        }
    }
}
