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

        public bool CreateGame(PlayerInfo host)
        {
            return BoardGameManager.CreateGame(host);
        }

        public GameRoom[] GetGameRooms()
        {
            return BoardGameManager.GetGameRooms();
        }

        public bool JoinGameRoom(PlayerInfo player, int gameId)
        {
            return BoardGameManager.JoinGameRoom(player, gameId);
        }
    }
}