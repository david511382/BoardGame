using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.GameFramework.GamePlayer
{
    public class GamePlayer
    {
        protected static int Player_Id;

        static GamePlayer()
        {
            Player_Id = 1;
        }

        public string Name { get { return Info.Name; } set { Info.Name = value; } }
        public int Id { get { return Info.Id; } }
        public PlayerInfo Info { get; private set; }

        protected BoardGame _game;
        protected PlayerResource _resource;

        public GamePlayer()
        {
            Info = new PlayerInfo($"Player{Player_Id}", Player_Id);
            Player_Id++;
            _game = null;
        }

        public GamePlayer(PlayerInfo player)
        {
            Info = new PlayerInfo(player.Name, player.Id);
            _game = null;
        }

        public  GameBoard GetGameTable() 
        {
            return _game.GetTable();
        }

        public void JoinGame<GameT>(ref GameT game) where GameT : BoardGame
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