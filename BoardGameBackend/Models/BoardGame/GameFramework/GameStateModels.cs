using BoardGame.Data.ApiParameters;
using BoardGame.Data.Enums;
using BoardGameBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.GameFramework
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

        public GameStatusModels Models(IPlayerManager playerConverter)
        {
            GameStatusModels model = new GameStatusModels();

            model.CurrentPlayerId = CurrentPlayerId;
            model.State = this.State;
            model.WinPlayers = playerConverter
                .GetPlayers(this.WinPlayerIds)
                .Select(d=>d.Models)
                .ToArray();

            return model;    
        }
    }
}