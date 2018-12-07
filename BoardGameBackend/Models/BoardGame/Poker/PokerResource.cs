using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame.PokerGame
{
    public class PokerResource : PlayerResource
    {
        List<PokerCard> _handCards;

        public PokerResource(int playerId)
            :base(playerId)
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

        public PokerCard[] GetHandCards()
        {
            return _handCards.ToArray();
        }
    }
}