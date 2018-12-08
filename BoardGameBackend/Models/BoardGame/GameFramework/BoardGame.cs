using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame.GameFramework
{
    public abstract partial class BoardGame<GamePlayerResource> where GamePlayerResource : PlayerResource
    {
        protected readonly int MAX_GAME_PLAYERS = 4;
        protected readonly int MIN_GAME_PLAYERS = 2;
        protected List<GamePlayerResource> _playerResources;
        protected GameBoard _table;
        protected int _currentTurn;

        protected int playerNum
        {
            get { return _playerResources.Count; }
        }

        public BoardGame(int maxPlayers, int minPlayers)
        {
            this.MAX_GAME_PLAYERS = maxPlayers;
            this.MIN_GAME_PLAYERS = minPlayers;

            _playerResources = new List<GamePlayerResource>();
            _table = new GameBoard();
        }

        public GameBoard GetTable()
        {
            return _table;
        }

        public GamePlayerResource GetResource(int playerId)
        {
            IEnumerable<GamePlayerResource> target = _playerResources.Where(d => d.PlayerId == playerId).Take(1);
            if (target.Count() == 0)
                throw new Exception("unknow player");

            return target.First();
        }

        protected virtual GamePlayerResource GetResourceAt(int i)
        {
            return _playerResources[i];
        }

        protected abstract void InitGame();
    }
}