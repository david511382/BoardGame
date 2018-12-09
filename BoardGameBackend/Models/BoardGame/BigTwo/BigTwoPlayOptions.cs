using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.BigTwo
{
    public partial class BigTwo : PokerGame.PokerGame
    {
        public bool PlayGroups(PokerCard[] cards)
        {
            //check cards group type
            //check cards playable
            //play
            _table.Put(new PokerCardGroup(cards));
            return true;
        }
    }
}