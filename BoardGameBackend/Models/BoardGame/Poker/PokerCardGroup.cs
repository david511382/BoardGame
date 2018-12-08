using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGame.Backend.Models.Game.BoardGame.PokerGame
{
    public partial class PokerCardGroup : GameObj
    {
        private PokerCard[] _cards;
        private PokerGroupType _groupType;
        private PokerCard _maxCard;

        public PokerCardGroup()
        {
            _maxCard = null;
        }

        public PokerCardGroup(PokerCard[] cards)
            :this()
        {
            if (cards == null)
                return;

            PokerGroupType type;
            try
            {
                type = GetCardGroupType(cards);
            }
            catch
            {
                return;
            }

            _groupType = type;
            _cards = cards;
            _maxCard = GetMaxValueOfCardGroup(_cards);
        }

        public PokerCard GetMaxValue()
        {
            return _maxCard;
        }

        public PokerGroupType GetGroupType()
        {
            if (_maxCard == null)
                throw new Exception("no type");

            return _groupType;
        }
    }
}