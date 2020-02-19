using GameLogic.Domain;
using System;

namespace GameLogic.Game
{
    public abstract partial class BoardGame<TBoardItem, TCondition> : IBoardGame where TBoardItem : GameObj where TCondition : GameStatus
    {
        public abstract void Load(string json);

        public abstract string ExportData();

        public void StartGame()
        {
            if (!IsGameOver())
                throw new Exception("game playing");

            if (_playerResources.Count < MIN_GAME_PLAYERS)
                throw new Exception("not enough players");

            currentTurn = 0;
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

        protected virtual void GameOver(int[] winnerId)
        {
            if (IsGameOver())
                throw new Exception("no game");

            _gameStaus.State = GameState.Game_Over;
            _gameStaus.WinPlayerIds = winnerId;

            GameOverNotifier?.Invoke();
        }

        protected abstract void AddPlayer(int playerId);

        public bool IsTurn(int id)
        {
            return GetResourceAt(currentTurn).PlayerId == id;
        }
    }
}