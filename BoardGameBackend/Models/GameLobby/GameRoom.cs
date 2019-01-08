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
        public int GameId { get; set; }
        public string HostName { get { return Players[0].Name; } }
        public int MaxPlayerCount { get; set; }
        public int MinPlayerCount { get; set; }
        public int CurrentPlayerCount { get { return Players.Count + 1; } }

        public GameRoomModels Models { get { return new GameRoomModels(GameId, Players[0].Models, MaxPlayerCount, MinPlayerCount); } }

        public List<PlayerInfo> Players { get; }

        public GameRoom(int gameId, PlayerInfo host, int maxPlayerCount, int minPlayerCount)
        {
            GameId = gameId;

            MaxPlayerCount = maxPlayerCount;
            MinPlayerCount = minPlayerCount;

            Players = new List<PlayerInfo>();
            Players.Add(host);
        }

        public bool AddPlayer(PlayerInfo player)
        {
            if (!IsFull())
                return false;

            Players.Add(player);

            return true;
        }

        public bool IsFull()
        {
            return CurrentPlayerCount >= MaxPlayerCount;
        }
    }
}