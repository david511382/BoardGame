﻿using GameLogic.Domain;
using System;
using System.Linq;

namespace GameLogic.Game
{
    public abstract partial class BoardGame<TBoardItem, TCondition> : IBoardGame where TBoardItem : GameObj
    {
        public abstract void Load(string json);

        public abstract string ExportData();

        public void StartGame()
        {
            if (!IsGameOver())
                throw new Exception("game playing");

            if (_playerResources.Count < MIN_GAME_PLAYERS)
                throw new Exception("not enough players");

            _currentTurn = 0;
            _gameStaus.State = GameState.OnTurn;

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

        public void Quit(int playerId)
        {
            if (!IsGameOver())
                throw new Exception("game playing");

            PlayerResource target = _playerResources
                .Where(d => d.PlayerId == playerId)
                .First();

            _playerResources.Remove(target);
        }

        public virtual void GameOver(int[] winnerId)
        {
            if (IsGameOver())
                throw new Exception("no game");

            _gameStaus.State = GameState.Game_Over;
            _gameStaus.WinPlayerIds = winnerId;

            GameOverNotifier?.Invoke();
        }

        public void RegisterGameOverEvent(Action caller)
        {
            GameOverNotifier = caller;
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