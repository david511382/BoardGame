using GameFramework.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BigTwo
{
    public partial class BigTwo
    {
        public static readonly PokerGroupType[] SUPER_GROUP_TYPE_ORDERS =
         {
             PokerGroupType.Four_Of_A_Kind,
             PokerGroupType.Straight_Flush,
             PokerGroupType.Dragon
        };

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

        public static int GetCompareValue(int number)
        {
            if (number <= BigTwo.MAX_CARD_NUMBER)
                number += Poker.NUMBER_NUM;
            return number;
        }
    }
}