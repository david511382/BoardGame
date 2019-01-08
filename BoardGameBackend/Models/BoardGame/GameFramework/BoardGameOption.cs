using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.GameFramework
{
    public abstract partial class BoardGame
    {
        public void StartGame()
        {
            if (!IsGameOver())
                throw new Exception("game playing");

            if (_playerResources.Count < MIN_GAME_PLAYERS)
                throw new Exception("not enough players");
            _currentTurn = 0;

            InitGame();
        }

        public void Join(int playerId)
        {
            if (!IsGameOver())
                throw new Exception("game playing");

            if (_playerResources.Count >= MAX_GAME_PLAYERS)
                throw new Exception("game full");

            AddPlayer(playerId);
        }

        protected abstract void AddPlayer(int playerId);

        public virtual void Surrender()
        {
            if (IsGameOver())
                throw new Exception("no game");

        }

        public GameStatus GetGameStatus()
        {
            return _gameStaus;
        }

        public bool IsTurn(int id)
        {
            return GetResourceAt(_currentTurn).PlayerId == id;
        }
    }
}