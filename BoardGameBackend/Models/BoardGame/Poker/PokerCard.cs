using BoardGame.Backend.Models.BoardGame;
using BoardGame.Backend.Models.BoardGame.GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.PokerGame
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
    }
}