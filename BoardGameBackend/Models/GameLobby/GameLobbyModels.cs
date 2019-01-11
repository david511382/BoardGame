using BoardGame.Backend.Models.BoardGame;
using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Data.ApiParameters;
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

        public GameRoom[] GetGameRooms()
        {
            GameRoom[] gameRooms = BoardGameManager.GetGameRooms();
            if (gameRooms.Length == 0)
                return null;

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
    }
}