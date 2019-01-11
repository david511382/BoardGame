using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Data.ApiParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.GameLobby
{
    public class GameRoom<GameT, PlayerT>
        where GameT : BoardGame.GameFramework.BoardGame
        where PlayerT : GamePlayer
    {
        public int RoomId { get; set; }
        public string HostName { get { return GamePlayers[0].Name; } }
        public int MaxPlayerCount { get; set; }
        public int MinPlayerCount { get; set; }
        public int CurrentPlayerCount { get { return GamePlayers.Count; } }

        public int HostId { get { return GamePlayers.First().Id; } }

        private GameT _game;

        public GameRoomModels Models
        {
            get
            {
                return new GameRoomModels(
                    RoomId,
                    GamePlayers.Select(d => d.Info.Models).ToArray(),
                    MaxPlayerCount,
                    MinPlayerCount);
            }
        }

        public List<PlayerT> GamePlayers { get; private set; }

        public GameRoom(GameT game, int roomId, ref PlayerInfo host, int maxPlayerCount, int minPlayerCount)
        {
            _game = game;

            RoomId = roomId;

            MaxPlayerCount = maxPlayerCount;
            MinPlayerCount = minPlayerCount;

            host.IsHost = true;

            GamePlayers = new List<PlayerT>();
            AddPlayer(ref host);
        }

        public bool Start()
        {
            try
            {
                foreach (PlayerT gamePlayer in GamePlayers)
                    gamePlayer.JoinGame(ref _game);

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
            foreach (PlayerT gamePlayer in GamePlayers)
                gamePlayer.QuitGame();
        }

        public PlayerT GetGamePlayer(int playerId)
        {
            PlayerT gp = GamePlayers
                .Where(d => d.Id == playerId)
                .First();

            return gp;
        }

        public bool AddPlayer(ref PlayerInfo player)
        {
            if (IsFull())
                return false;

            player.JoinRoom(RoomId);

            PlayerT gamePlayer = (PlayerT)_game.CreatePlayer(player);
            GamePlayers.Add(gamePlayer);

            return true;
        }

        public void LeavePlayer(ref PlayerInfo player)
        {
            int playerId = player.Id;
            PlayerT gamePlayer = GamePlayers
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

        public bool IsEnoughPlayer()
        {
            return CurrentPlayerCount >= MinPlayerCount;
        }

        public GameT GetGame()
        {
            return _game;
        }
    }
}