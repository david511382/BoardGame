using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Game
{
    public class GameBoard<TItem> where TItem : GameObj
    {
        public List<TItem> Items;

        public GameBoard()
        {
            Items = new List<TItem>();
        }

        public void Put(TItem item)
        {
            Items.Add(item);
        }

        public TItem GetLastItem()
        {
            return Items.LastOrDefault();
        }
    }
}