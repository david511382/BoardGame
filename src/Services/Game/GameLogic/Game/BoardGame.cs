using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Domain;
using GameLogic.Player;

namespace GameLogic.Game
{
    public abstract partial class BoardGame
    {
        protected readonly int MAX_GAME_PLAYERS = 4;
        protected readonly int MIN_GAME_PLAYERS = 2;

        protected int currentTurn
        {
            get { return _currentTurn; }
            set
            {
                _currentTurn =(value >= _playerResources.Count) ?
                    0 :
                    value;

                _gameStaus.CurrentPlayerId = GetResourceAt(_currentTurn).PlayerId;
            }
        }

        protected List<PlayerResource> _playerResources;
        protected GameBoard _table;
        private int _currentTurn;
        protected GameStatus _gameStaus;

        private Action GameOverNotifier;

        protected int playerNum
        {
            get { return _playerResources.Count; }
        }

        public BoardGame(int maxPlayers, int minPlayers)
        {
            this.MAX_GAME_PLAYERS = maxPlayers;
            this.MIN_GAME_PLAYERS = minPlayers;

            _playerResources = new List<PlayerResource>();
            _table = new GameBoard();
            _gameStaus = new GameStatus(GameState.Game_Over);
        }

        public GameBoard GetTable()
        {
            return _table;
        }

        public PlayerResource GetResource(int playerId)
        {
            IEnumerable<PlayerResource> target = _playerResources.Where(d => d.PlayerId == playerId).Take(1);
            if (target.Count() == 0)
                throw new Exception("unknow player");

            return target.First();
        }

        public T GetResource<T>(int playerId) where T : PlayerResource
        {
            var target = _playerResources.Where(d => d.PlayerId == playerId).Take(1);
            if (target.Count() == 0)
                throw new Exception("unknow player");

            return (T)target.First();
        }

        public abstract GamePlayer CreatePlayer(PlayerInfo playerInfo);

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