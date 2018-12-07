using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame
{
    public class GamePlayer<Resource> where Resource : PlayerResource
    {
        protected static int Player_Id;
        protected BoardGame.BoardGame<Resource> _game;

        static GamePlayer()
        {
            Player_Id = 1;
        }

        public string Name;
        public int Id;

        protected PlayerResource _resource;

        public GamePlayer()
        {
            Id = Player_Id;
            Name = $"Player {Id}";
            Player_Id++;
        }

        public GamePlayer(GamePlayer<Resource> gamePlayer)
        {
            Id = gamePlayer.Id;
            Name = gamePlayer.Name;
        }

        public void JoinGame(BoardGame.BoardGame<Resource> game) 
        {
            game.Join(Id);
            _game = game;
        }
    }
}