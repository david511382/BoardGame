using BoardGame.Backend.Models.BoardGame;
using BoardGame.Data.ApiParameters;
using GameLogic.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.GameLobby
{
    public class GameLobbyModels
    {
        public PlayerInfo Register()
        {
            return BoardGameManager.Register();
        }

        public PlayerInfo GetPlayer(PlayerInfo user)
        {
            try
            {
                return BoardGameManager.GetPlayerById(user.Id);
            }
            catch
            {
                return null;
            }
        }

        public PlayerInfo CreateGame(PlayerInfo host)
        {
            return BoardGameManager.CreateGame(host);
        }

        public GameRoomModel[] GetGameRooms()
        {
            GameRoomModel[] gameRooms = BoardGameManager.GetGameRooms()
                .Select(d => d.Models)
                .ToArray();

            if (gameRooms.Length == 0)
                return new GameRoomModel[0];

            return gameRooms;
        }

        public PlayerInfo JoinGameRoom(PlayerInfo player, int gameId)
        {
            return BoardGameManager.JoinGameRoom(player, gameId);
        }

        public PlayerInfo[] LeaveGameRoom(PlayerInfo player)
        {
            return BoardGameManager.LeaveGameRoom(player);
        }

        public bool StartGame(PlayerInfo player)
        {
            return BoardGameManager.StartGame(player);
        }
    }
}