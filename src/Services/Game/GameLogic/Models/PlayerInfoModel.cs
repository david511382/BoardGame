using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Models
{
    public class PlayerInfoModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("IsInRoom")]
        public bool IsInRoom { get; set; }

        [JsonProperty("RoomID")]
        public int RoomId { get; set; }

        [JsonProperty("IsHost")]
        public bool IsHost { get; set; }

        public PlayerInfoModel()
        {
        }

        public PlayerInfoModel(string name, int id)
            : this(name, id, -1, false)
        {
        }

        public PlayerInfoModel(string name, int id, int roomId, bool isHost)
        {
            this.Name = name;
            this.Id = id;
            RoomId = roomId;
            IsInRoom = RoomId != -1;
            IsHost = isHost;
        }
    }
}
