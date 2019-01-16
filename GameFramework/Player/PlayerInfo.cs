using BoardGame.Data.ApiParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFramework.Player
{
    public class PlayerInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsInRoom { get { return RoomId != -1; } }
        public int RoomId { get; private set; }
        public bool IsHost { get; set; }
        public PlayerInfoModels Models { get { return new PlayerInfoModels(Name, Id, RoomId, IsHost); } }

        public PlayerInfo(string name, int id)
        {
            this.Name = name;
            this.Id = id;
            RoomId = -1;
        }

        public PlayerInfo(PlayerInfoModels models)
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