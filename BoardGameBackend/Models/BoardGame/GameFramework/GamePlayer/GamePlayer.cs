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

        public string Name { get { return Info.Name; } set { _info.Name = value; } }
        public int Id { get { return Info.Id; } }
        public PlayerInfo Info { get { return _info; } }

        protected BoardGame _game;
        protected PlayerResource _resource;

        private PlayerInfo _info;

        public GamePlayer()
        {
            _info = new PlayerInfo($"Player {Id}", Player_Id);
            Player_Id++;
        }

        public GamePlayer(GamePlayer gamePlayer)
        {
            _info = new PlayerInfo(gamePlayer.Name, gamePlayer.Id);
        }

        public  GameBoard GetGameTable() 
        {
            return _game.GetTable();
        }

        public void JoinGame(BoardGame game)
        {
            game.Join(Id);
            _game = game;
        }
    }
}