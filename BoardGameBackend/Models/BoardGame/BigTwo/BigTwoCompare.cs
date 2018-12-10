using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.BigTwo
{
    public partial class BigTwo
    {
        private static int CompareCard(PokerCard a, PokerCard b)
        {
            int aNum = a.Number + ((a.Number <= MAX_CARD_NUMBER) ? Poker.NUMBER_NUM : 0);
            int bNum = b.Number + ((b.Number <= MAX_CARD_NUMBER) ? Poker.NUMBER_NUM : 0);

            if (aNum > bNum)
                return 1;
            else if (aNum < bNum)
                return -1;
            else
            {
                if (a.Suit > b.Suit)
                    return 1;
                else if (a.Suit < b.Suit)
                    return -1;
                else
                    return 0;
            }
        }
    }
}