using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame
{
    public abstract partial class BoardGame<GamePlayerResource> where GamePlayerResource : PlayerResource
    {
        public void StartGame()
        {
            if (_playerResources.Count < MIN_GAME_PLAYERS)
                throw new Exception("not enough players");
            _currentTurn = 0;

            InitGame();
        }

        public void Join(int playerId)
        {
            if (_playerResources.Count >= MAX_GAME_PLAYERS)
                throw new Exception("game full");

            AddPlayer(playerId);
        }

        protected abstract void AddPlayer(int playerId);

        public void Surrender()
        {
            throw new NotImplementedException();
        }

        public bool IsTurn(int id)
        {
            return GetResourceAt(_currentTurn).PlayerId == id;
        }
    }
}