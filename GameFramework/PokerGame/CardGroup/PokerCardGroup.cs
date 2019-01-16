using GameFramework.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GameFramework.PokerGame.CardGroup
{
    public partial class PokerCardGroup : GameObj
    {
        public enum SuitEqul
        {
            same,
            notSame,
            dontCare
        }

        private PokerCard[] _cards;
        private PokerGroupType _groupType;
        private PokerCard _maxCard;

        public PokerCardGroup()
        {
            _maxCard = null;
        }

        public PokerCardGroup(PokerCard[] cards)
            : this()
        {
            if (cards == null)
                return;

            PokerGroupType type;
            try
            {
                type = GetMaxCardGroupType(cards);
            }
            catch
            {
                return;
            }

            _groupType = type;
            _cards = cards;
            _maxCard = GetMaxValueOfCardGroup<CardGroupModel>(_cards);
        }

        public PokerCard GetMaxValue()
        {
            return _maxCard;
        }

        public PokerCard[] GetCards()
        {
            return _cards;
        }

        public PokerGroupType GetGroupType()
        {
            if (_maxCard == null)
                throw new Exception("no type");

            return _groupType;
        }
    }
}