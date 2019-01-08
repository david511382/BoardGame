using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.GameFramework
{
    public struct GameStatus
    {
        public enum GameState
        {
            Game_Over,
            OnTurn
        };

        public GameState State { get; set; }
        public int[] WinPlayerIds { get; set; }
        public int CurrentPlayerId { get; set; }

        public GameStatus(GameState state)
        {
            this.State = state;
            WinPlayerIds = null;
            CurrentPlayerId = -1;
        }
    }
}