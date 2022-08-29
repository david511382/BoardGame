using GameLogic.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Game
{
    public abstract partial class BoardGame<TBoardItem, TCondition> : IBoardGame where TBoardItem : GameObj where TCondition : GameStatus
    {
        protected readonly int MAX_GAME_PLAYERS = 4;
        protected readonly int MIN_GAME_PLAYERS = 2;

        protected int currentTurn
        {
            get { return _gameStaus.TurnId; }
            set
            {
                _gameStaus.TurnId = (value >= _playerResources.Count) ?
                    0 :
                    value;

                _gameStaus.CurrentPlayerId = GetResourceAt(_gameStaus.TurnId).PlayerId;
            }
        }

        protected List<PlayerResource> _playerResources;
        protected GameBoard<TBoardItem> Table;
        protected GameStatus _gameStaus;

        private Action GameOverNotifier;

        protected int playerNum
        {
            get { return _playerResources.Count; }
        }

        public BoardGame(int maxPlayers, int minPlayers)
        {
            MAX_GAME_PLAYERS = maxPlayers;
            MIN_GAME_PLAYERS = minPlayers;

            _playerResources = new List<PlayerResource>();
            Table = new GameBoard<TBoardItem>();
            _gameStaus = new GameStatus(GameState.Game_Over);
        }

        public GameBoard<TBoardItem> GetTable()
        {
            return Table;
        }

        public virtual GameStatus GetCondition()
        {
            return _gameStaus;
        }

        public virtual PlayerResource GetResource(int playerId)
        {
            IEnumerable<PlayerResource> target = _playerResources.Where(d => d.PlayerId == playerId).Take(1);
            if (target.Count() == 0)
                throw new Exception("unknow player");

            return target.First();
        }

        protected virtual PlayerResource GetResourceAt(int i)
        {
            return _playerResources[i];
        }

        protected abstract void InitGame();

        protected bool IsGameOver()
        {
            return _gameStaus.State == GameState.Game_Over;
        }
    }
}