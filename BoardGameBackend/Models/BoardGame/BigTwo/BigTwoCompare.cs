using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.BigTwo
{
    public partial class BigTwo
    {
        public static int CompareCard(PokerCard a, PokerCard b)
        {
            int compareNumber = CompareNumber(a.Number, b.Number);
            if (compareNumber == 0)
            {
                if (a.Suit > b.Suit)
                    return 1;
                else if (a.Suit < b.Suit)
                    return -1;
                else
                    return 0;
            }
            else
                return compareNumber;
        }
        
        public static int CompareNumber(int a, int b)
        {
            bool aIsMax = a == MAX_CARD_NUMBER;
            bool bIsMax = b == MAX_CARD_NUMBER;

            if (aIsMax && !bIsMax)
                return 1;
            else if (aIsMax && bIsMax)
                return 0;
            else if (!aIsMax && bIsMax)
                return -1;
            else
                return Poker.Compare_Number(a, b);
        }
    }
}