using GameFramework.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFramework.PokerGame
{
    public class PokerCard : GameObj
    {
        public int Number
        {
            get
            {
                return this._number;
            }
            set
            {
                if (value > 0 && value < 53)
                    this._number = value;
                else if (this._number <= 0 || this._number >= 53)
                    this._number = 1;
            }
        }
        public PokerSuit Suit { get; set; }

        private int _number;

        public PokerCard()
        {
            this.Number = 1;
            this.Suit = PokerSuit.Spade;
        }

        public PokerCard(PokerSuit suit, int number)
        {
            this.Number = number;
            this.Suit = suit;
        }

        public bool isSame(PokerCard c)
        {
            return this.Number == c.Number && this.Suit == c.Suit;
        }

        public static int CompareCard(PokerCard a, PokerCard b)
        {
            int compareNumber = Poker.Compare_Number(a.Number, b.Number);
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
    }
}