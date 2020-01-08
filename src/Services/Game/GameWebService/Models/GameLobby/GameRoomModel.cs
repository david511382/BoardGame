using GameLogic.Models;
using Newtonsoft.Json;

namespace GameWebService.Models.GameLobby
{
    public class GameRoomModel
    {
        [JsonProperty("RoomID")]
        public int RoomId { get; set; }

        [JsonProperty("HostName")]
        public string HostName { get; set; }

        [JsonProperty("MaxPlayerCount")]
        public int MaxPlayerCount { get; set; }

        [JsonProperty("MinPlayerCount")]
        public int MinPlayerCount { get; set; }

        [JsonProperty("CurrentPlayerCount")]
        public int CurrentPlayerCount { get; set; }

        [JsonProperty("Players")]
        public PlayerInfoModel[] Players { get; set; }

        public GameRoomModel()
        {

        }

        public GameRoomModel(int roomId, PlayerInfoModel[] players, int maxPlayerCount, int minPlayerCount)
        {
            RoomId = roomId;

            MaxPlayerCount = maxPlayerCount;
            MinPlayerCount = minPlayerCount;

            Players = players;
            HostName = Players[0].Name;
            CurrentPlayerCount = Players.Length;
        }
    }
}
