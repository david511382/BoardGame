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
        public string HostName { get { return GamePlayers[0].Name; } }
        public int MaxPlayerCount { get; set; }
        public int MinPlayerCount { get; set; }
        public int CurrentPlayerCount { get { return GamePlayers.Count; } }

        public int HostId { get { return GamePlayers.First().Id; } }

        private BoardGame.GameFramework.BoardGame _game;

        public GameRoomModels Models { get
        {
            return new GameRoomModels(
                RoomId,
                GamePlayers.Select(d=>d.Info.Models).ToArray(), 
                MaxPlayerCount, 
                MinPlayerCount);
            }
        }

        public List<GamePlayer> GamePlayers { get; }

        public GameRoom(BoardGame.GameFramework.BoardGame game, int roomId, ref PlayerInfo host, int maxPlayerCount, int minPlayerCount)
        {
            _game = game;

            RoomId = roomId;

            MaxPlayerCount = maxPlayerCount;
            MinPlayerCount = minPlayerCount;

            host.IsHost = true;

            GamePlayers = new List<GamePlayer>();
            AddPlayer(ref host);
        }

        public bool Start()
        {
            foreach(GamePlayer gamePlayer in GamePlayers)
                gamePlayer.JoinGame(ref _game);

            try
            {
                _game.StartGame();
                return true;
            }
            catch
            {
                return false;   
            }
        }

        public void GameOver()
        {
            foreach (GamePlayer gamePlayer in GamePlayers)
                gamePlayer.QuitGame();
        }

        public GamePlayer GetGamePlayer(int playerId)
        {
            return GamePlayers.Where(d => d.Id == playerId).First();
        }

        public bool AddPlayer(ref PlayerInfo player)
        {
            if (IsFull())
                return false;

            GamePlayer gamePlayer = new GamePlayer(player);
            GamePlayers.Add(gamePlayer);

            player.JoinRoom(RoomId);

            return true;
        }

        public void LeavePlayer(ref PlayerInfo player)
        {
            int playerId = player.Id;
            GamePlayer gamePlayer = GamePlayers
                .Where(d => d.Id == playerId)
                .First();
            GamePlayers.Remove(gamePlayer);

            player.LeaveRoom();

            if (CurrentPlayerCount == 0)
            {
                //close room
            }
            else
            {
                // change host
                gamePlayer = GamePlayers.First();
                gamePlayer.Info.IsHost = true;
            }
        }

        public bool IsFull()
        {
            return CurrentPlayerCount >= MaxPlayerCount;
        }

        public BoardGame.GameFramework.BoardGame GetGame()
        {
            return _game;
        }
    }
}