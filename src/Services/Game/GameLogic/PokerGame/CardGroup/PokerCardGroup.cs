using GameLogic.Game;
using System;

namespace GameLogic.PokerGame.CardGroup
{
    public partial class PokerCardGroup : GameObj
    {
        public enum SuitEqul
        {
            same,
            notSame,
            dontCare
        }

        public PokerCard[] Cards;
        public PokerGroupType GroupType;
        public PokerCard MaxCard;

        public PokerCardGroup()
        {
            MaxCard = null;
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

            GroupType = type;
            Cards = cards;
            MaxCard = GetMaxValueOfCardGroup<CardGroupModel>(Cards);
        }

        public PokerCard GetMaxValue()
        {
            return MaxCard;
        }

        public PokerCard[] GetCards()
        {
            return Cards;
        }

        public PokerGroupType GetGroupType()
        {
            if (MaxCard == null)
                throw new Exception("no type");

            return GroupType;
        }
    }
}