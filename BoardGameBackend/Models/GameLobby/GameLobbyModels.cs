using BoardGame.Backend.Models.BoardGame;
using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.GameLobby
{
    public class GameLobbyModels
    {
        public PlayerInfo CreateGame()
        {
            return BoardGameManager.CreateGame();
        }
    }
}