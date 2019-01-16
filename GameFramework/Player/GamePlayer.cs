using GameFramework.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFramework.Player
{
    public class GamePlayer
    {
        public string Name { get; set; }
        public int Id { get; }

        protected Game.BoardGame _game;
        protected PlayerResource _resource;

        public GamePlayer(int id, string Name)
        {
            Id = id;
            this.Name = Name;
            _game = null;
        }

        public  GameBoard GetGameTable() 
        {
            return _game.GetTable();
        }

        public void JoinGame<GameT>(ref GameT game) where GameT : Game.BoardGame
        {
            _game = game;
            game.Join(Id);
        }

        public void QuitGame()
        {
            if (_game != null)
            {
                _game.Quit(Id);
                _game = null;
            }
        }
    }
}