using GameLogic.Models;

namespace GameLogic.Player
{
    public class PlayerInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsInRoom { get { return RoomId != -1; } }
        public int RoomId { get; private set; }
        public bool IsHost { get; set; }
        public PlayerInfoModel Models { get { return new PlayerInfoModel(Name, Id, RoomId, IsHost); } }

        public PlayerInfo(string name, int id)
        {
            Name = name;
            Id = id;
            RoomId = -1;
        }

        public PlayerInfo(PlayerInfoModel models)
            : this(models.Name, models.Id)
        {
        }

        public void JoinRoom(int roomId)
        {
            RoomId = roomId;
        }

        public void LeaveRoom()
        {
            RoomId = -1;
            IsHost = false;
        }
    }
}