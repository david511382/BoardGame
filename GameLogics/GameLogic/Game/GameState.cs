using BoardGame.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameLogic.Game
{
    public class GameStatus
    {
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