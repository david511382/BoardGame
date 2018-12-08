﻿using BoardGame.Backend.Models.Game.BoardGame.PokerGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame.BigTwo
{
    public partial class BigTwo : PokerGame.PokerGame
    {
        public const int MAX_PLAYERS = 4;
        public const int MIN_PLAYERS = 4;
        public const int MAX_CARD_NUMBER = 2;

        private bool _isFreeType;

        public BigTwo()
            : base(MAX_PLAYERS, MIN_PLAYERS)
        {
            _isFreeType = true;
        }

        protected override void InitGame()
        {
            base.InitGame();

            PokerCard club3 = new PokerCard(PokerSuit.Club, 3);
            for(int i = 0; i < _playerResources.Count; i++)
            {
                if (_playerResources[i].GetHandCards().Where(d=>d.Suit == club3.Suit && d.Number == club3.Number).Count()>0)
                {
                    _currentTurn = i;
                    break;
                }
            }
        }

        public bool IsFreeType()
        {
            return _isFreeType;
        }

        private bool IsCLub3(PokerCard card)
        {
            return (card.Number == 3) && (card.Suit == PokerSuit.Club);
        }
    }
}