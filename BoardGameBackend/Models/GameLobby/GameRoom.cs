using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Data.ApiParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.GameLobby
{
    public class GameRoom
    {
        public int RoomId { get; set; }
        public string HostName { get { return Players[0].Name; } }
        public int MaxPlayerCount { get; set; }
        public int MinPlayerCount { get; set; }
        public int CurrentPlayerCount { get { return Players.Count; } }

        public GameRoomModels Models { get { return new GameRoomModels(RoomId, Players[0].Models, MaxPlayerCount, MinPlayerCount); } }

        public List<PlayerInfo> Players { get; }

        public GameRoom(int gameId, PlayerInfo host, int maxPlayerCount, int minPlayerCount)
        {
            RoomId = gameId;

            MaxPlayerCount = maxPlayerCount;
            MinPlayerCount = minPlayerCount;

            Players = new List<PlayerInfo>();
            AddPlayer(host);
        }

        public bool AddPlayer(PlayerInfo player)
        {
            if (IsFull())
                return false;

            Players.Add(player);
            player.JoinRoom(RoomId);

            return true;
        }

        public bool IsFull()
        {
            return CurrentPlayerCount >= MaxPlayerCount;
        }
    }
}