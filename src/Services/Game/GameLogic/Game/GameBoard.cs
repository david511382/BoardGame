using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameLogic.Game
{
    public class GameBoard
    {
        List<GameObj> _items;

        public GameBoard()
        {
            _items = new List<GameObj>();
        }

        public void Put(GameObj item)
        {
            _items.Add(item);
        }

        public GameObj GetLastItem()
        {
            return _items.LastOrDefault();
        }
    }
}