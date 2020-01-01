namespace Domain.Api.Models.Base.Lobby
{
    public class RoomModel
    {
        public int HostID { get; set; }
        public GameModel Game { get; set; }
        public int[] PlayerIDs { get; set; }

        public bool IsFull()
        {
            return PlayerIDs.Length == Game.MaxPlayerCount;
        }
    }
}
