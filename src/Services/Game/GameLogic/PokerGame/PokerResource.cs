﻿using GameLogic.Game;
using System.Collections.Generic;

namespace GameLogic.PokerGame
{
    public class PokerResource : PlayerResource
    {
        public List<PokerCard> _handCards;

        public PokerResource(int playerId)
            : base(playerId)
        {
            _handCards = new List<PokerCard>();
        }

        public void SetHandCard(PokerCard[] cards)
        {
            _handCards.Clear();
            _handCards.AddRange(cards);
        }

        public void AddHandCard(PokerCard card)
        {
            _handCards.Add(card);
        }

        public void RemoveHandCards(PokerCard[] card)
        {
            for (int i = 0; i < card.Length; i++)
            {
                for (int j = 0; j < _handCards.Count; j++)
                {
                    if (_handCards[j].isSame(card[i]))
                    {
                        _handCards.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        public PokerCard[] GetHandCards()
        {
            return _handCards.ToArray();
        }
    }
}