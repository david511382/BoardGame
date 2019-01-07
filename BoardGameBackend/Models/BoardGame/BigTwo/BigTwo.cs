using BoardGame.Backend.Models.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.BoardGame.BigTwo
{
    public partial class BigTwo : PokerGame.PokerGame
    {
        public const int MAX_PLAYERS = 4;
        public const int MIN_PLAYERS = 4;
        public const int MAX_CARD_NUMBER = 2;

        public static readonly PokerCard CLUB_3;

        public bool IsFreeType { get; private set; }
        public bool IsRequiredClub3 { get; private set; }

        private PokerResource CurrentPlayerResource
        {
            get { return GetResourceAt(_currentTurn); }
        }

        public static bool IsCLub3(PokerCard card)
        {
            return (card.Number == 3) && (card.Suit == PokerSuit.Club);
        }

        static BigTwo()
        {
            CLUB_3 = new PokerCard(PokerSuit.Club, 3);
        }

        public BigTwo()
            : base(MAX_PLAYERS, MIN_PLAYERS)
        {
            IsFreeType = true;
            IsRequiredClub3 = true;
        }

        protected override void InitGame()
        {
            base.InitGame();

            PokerCard club3 = new PokerCard(PokerSuit.Club, 3);
            for(int i = 0; i < _playerResources.Count; i++)
            {
                if (base.GetResourceAt(i).GetHandCards().Where(d=>d.Suit == club3.Suit && d.Number == club3.Number).Count()>0)
                {
                    _currentTurn = i;
                    _lastPlayTurnId = ((_currentTurn == 0) ?
                        _playerResources.Count :
                        _currentTurn)
                        - 1;
                    break;
                }
            }
        }

        private void NextTurn()
        {
            if (++_currentTurn >= _playerResources.Count)
                _currentTurn = 0;
        }
    }
}