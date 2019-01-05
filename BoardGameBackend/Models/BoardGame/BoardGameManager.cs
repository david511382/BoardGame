using BoardGame.Backend.Models.BoardGame.BigTwo;
using BoardGame.Backend.Models.BoardGame.GameFramework;
using BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer;
using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame
{
    public static class BoardGameManager
    {
        private static List<GameFramework.BoardGame> _games;
        private static List<GamePlayer> _gamePlayers;

        static BoardGameManager()
        {
            _games = new List<GameFramework.BoardGame>();
            _gamePlayers = new List<GamePlayer>();

            CreateGame();

            JoinGame();
            JoinGame();
            JoinGame();

            StartGame();
        }

        public static BigTwoPlayer GetPlayerById(int playerId)
        {
            try
            {
                return (BigTwoPlayer)_gamePlayers
                    .Where(d => d.Id == playerId)
                    .First();
            }
            catch
            {
                throw new Exception("no player");
            }
        }

        public static GameBoard GetGameBoardByPlayerId(int playerId)
        {
            try
            {
                 GamePlayer gp = _gamePlayers
                    .Where(d => d.Id == playerId)
                    .First();

                return gp.GetGameTable();
            }
            catch
            {
                throw new Exception("no player");
            }
        }

        public static PlayerInfo CreateGame()
        {
            BigTwo.BigTwo bigTwo = new BigTwo.BigTwo();
            _games.Add(bigTwo);

            return JoinGame();
        }

        public static PlayerInfo JoinGame()
        {
            BigTwoPlayer player = new BigTwoPlayer();
            _gamePlayers.Add(player);
            player.JoinGame(_games.Last());
            return player.Info;
        }

        public static bool StartGame()
        {
            try
            {
                _games.Last().StartGame();
                return true;
            }
            catch { return false; }
        }
    }
}