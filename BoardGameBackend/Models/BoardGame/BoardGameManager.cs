using BoardGame.Backend.Models.Game.BoardGame.BigTwo;
using BoardGame.Backend.Models.Game.BoardGame.GameFramework;
using BoardGame.Backend.Models.Game.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGameBackend.Models.BoardGame
{
    public static class BoardGameManager
    {
        public static BigTwoPlayer _thisPlayer;

        static BoardGameManager()
        {
            BigTwo bigTwo = new BigTwo();
            _thisPlayer = new BigTwoPlayer();
            _thisPlayer.JoinGame(bigTwo);

            GamePlayer<PokerResource> bigTwoP2, bigTwoP3, bigTwoP4;
            bigTwoP2 = new BigTwoPlayer();
            bigTwoP3 = new BigTwoPlayer();
            bigTwoP4 = new BigTwoPlayer();

            bigTwoP2.JoinGame(bigTwo);
            bigTwoP3.JoinGame(bigTwo);
            bigTwoP4.JoinGame(bigTwo);

            bigTwo.StartGame();
        }
    }
}